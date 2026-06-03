using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DTO;
using Model;
using Controller;
using System.Drawing.Printing;
using System.Drawing;
using System.Text.RegularExpressions;
using System.IO;
using ZXing;
using System.Drawing.Drawing2D;

namespace ErpClass
{
    /// <summary>
    /// Lógica de interacción para MyModal.xaml
    /// </summary>
    public partial class ModalVenta : MetroWindow
    {
        readonly ModelDataBase P = new ModelDataBase();
        readonly InformeController controller = new InformeController();
        public int descuento = 0;
        public int total_descuento = 0;
        public ModalVenta()
        {

            InitializeComponent();
            if (App.Current.MainWindow.IsActive)
            {
                this.Owner = App.Current.MainWindow;
            }
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowStyle = WindowStyle.None;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.ShowCloseButton = false;
            cargarDatos();
            int? total_desc = GlobalClass.productos.Sum(x => x.precio_desc);
            if (total_desc != null)
            {
                tb_descuento.Content = String.Format("{0:N0}", total_desc);
            }

            tb_porpagar.Content = String.Format("{0:N0}", GlobalClass.total);
            tb_total_a_pagar.Content = String.Format("{0:N0}", GlobalClass.total);
            tb_total_pagado.Content = String.Format("{0:N0}", GlobalClass.saldo_devolucion);
            tb_total_pagado.Content = String.Format("{0:N0}", GlobalClass.saldo_devolucion);
            if (GlobalClass.saldo_devolucion != null)
            {
                lbl_saldo_devo.Visibility = Visibility.Visible;
                lbl_sobrante.Visibility = Visibility.Visible;
                tb_saldo_favor.Visibility = Visibility.Visible;
                tb_saldo_sobrante.Visibility = Visibility.Visible;

                tb_saldo_favor.Content = String.Format("{0:N0}", GlobalClass.saldo_devolucion);
                calcularPago();
            }
            lb_subtotal.Content = String.Format("{0:N0}", GlobalClass.total);
        }
        public void cargarDatos()
        {
            grid_venta.ItemsSource = GlobalClass.productos;

        }
        private void bt_realizarventa_Click(object sender, RoutedEventArgs e)
        {
            calcularPago();
            bool val = true;
            int numtotalpago = 0;
            if (getNumber(tb_total_pagado) == 0)
            {
                val = false;
                msj("Debe ingresar montos de pago");
            }
            else if (check_factura.IsChecked == true)
            {
                if (GlobalClass.factura == null)
                {
                    val = false;
                    msj("Debe ingresar datos de factura");
                }
            }

            if (val)
            {

                int totalventa = getNumber(tb_total_a_pagar);
                numtotalpago = getNumber(tb_total_pagado);

                int redcompra = getNumber(tb_redcompra);
                int efectivo = getNumber(tb_efectivo);
                int transferencia = getNumber(tb_transferencia);

                bool valmediopago = true;
                if (numtotalpago >= totalventa)
                {
                    int total = redcompra + transferencia;

                    if (total > 0 && numtotalpago > totalventa)
                    {
                        msj("MEDIO DE PAGO MAYOR A LA VENTA", null, true);
                        valmediopago = false;
                    }
                    if (valmediopago)
                    {

                        Pedido pedido = P.Pedido.FirstOrDefault(x => x.codigo == GlobalClass.idpedido);

                        if (pedido == null)
                        {
                            msj("VENTA YA REALIZADA", null, true);
                            this.Close();
                        }
                        else
                        {
                            var resulme = msj($"¿DESEA CONFIRMAR EL PAGO?", true);
                            if (resulme == true)
                            {
                                pedido.precio_total = totalventa;

                                pedido.fecha = DateTime.Now;
                                pedido.idestado = 2;

                                pedido.idComputador = GlobalClass.IdComputador;

                                pedido.descuento = total_descuento;

                                List<AddIngDTO> ingDTOs = new List<AddIngDTO>();
                                foreach (DetalleDTO item in grid_venta.Items)
                                {

                                    actualizarStock(item.codigo, item.idinventario, item.Cantidad);
                                }
                                int v = 0;
                                v = P.SaveChanges();

                                GlobalClass.idcliente = null;
                                GlobalClass.nombre = "";

                                if (v > 0)
                                {
                                    GenerarFactura();
                                    guardarMovVenta(pedido.codigo);
                                    printTicket(pedido.codigo);
                                    if (pedido.despacho == "des" || pedido.despacho == "ret48")
                                    {
                                        getDespacho(pedido.codigo);
                                    }
                                    if (GlobalClass.idventadevolucion != null)
                                    {
                                        if (getNumber(tb_saldo_sobrante) > 0)
                                        {
                                            devolucion();
                                        }
                                        if (getNumber(tb_saldo_sobrante) == 0)
                                        {
                                            cambiarEstadoDevolucion();
                                        }
                                    }

                                    GlobalClass.productos.Clear();
                                    GlobalClass.estado = 1;
                                    GlobalClass.saldo_devolucion = null;
                                    GlobalClass.idventadevolucion = null;
                                    this.Close();
                                }
                                else
                                {
                                    msj("Error en la base de datos");
                                }
                            }
                        }

                    }

                }
                else
                {
                    msj("Falta monto por pagar");
                }
            }
        }
        private void GenerarFactura()
        {
            if (check_factura.IsChecked == true)
            {
                try
                {
                    controller.CrearFactura(new FacturaDTO
                    {
                        idpedido = GlobalClass.idpedido,
                        rut = GlobalClass.factura.rut,
                        numero = GlobalClass.factura.numero,
                        correo = GlobalClass.factura.correo,
                        fecha = DateTime.Now,
                        estado = 0
                    });
                    DialogResult = true;
                    this.Close();
                }
                catch (Exception ex)
                {
                    ModalMensaje m = new ModalMensaje(ex.Message, null, true);
                    m.Show();
                }
            }


        }
        private void printTicket(int codigo)
        {
            string comanda = GetBoleta();


            string codpedido = codigo.ToString();
            string[] lines = comanda.ToString().Split(Environment.NewLine.ToCharArray());

            int lineas = (lines.Length * 20) + 300;
            string textoFinal = "El PEDIDO ES HASTA DONDE VEHICULO PUEDA INGRESAR";
            PrintDocument p = new PrintDocument();
            PrinterSettings settings = new PrinterSettings();
            PaperSize paperSize = new PaperSize("Ticket", 300, lineas);
            settings.DefaultPageSettings.PaperSize = paperSize;

            p.PrinterSettings = settings;
            p.PrintPage += delegate (object sender1, PrintPageEventArgs e1)
            {
                var sri = Application.GetResourceStream(new Uri("pack://application:,,,/Recursos/logo.ico"));
                var img = System.Drawing.Image.FromStream(sri.Stream);


                RectangleF rectanguloLogo = new RectangleF((e1.PageBounds.Width - 120) / 2, 0,
                    120,
                    120);

                e1.Graphics.DrawImage(img, rectanguloLogo);

                Font font = new Font("Arial Bold", 10);
                SizeF stringSize = new SizeF();
                stringSize = e1.Graphics.MeasureString(comanda, font, e1.PageBounds.Width);

                RectangleF rectanguloTexto = new RectangleF(0, rectanguloLogo.Bottom,
                    stringSize.Width,
                   stringSize.Height);

                e1.Graphics.DrawString(comanda,
                    font,
                    new SolidBrush(System.Drawing.Color.Black),
                    rectanguloTexto);

                e1.Graphics.DrawString(textoFinal,
    new Font("Arial", 7, System.Drawing.FontStyle.Bold, GraphicsUnit.Point),
    new SolidBrush(System.Drawing.Color.Black),
    0, (int)rectanguloTexto.Bottom + 20);

                System.Drawing.Rectangle rectanguloQr =
                new System.Drawing.Rectangle(
                    (e1.PageBounds.Width - 150) / 2,
                    (int)rectanguloTexto.Bottom + 40,
                   150,
                   150);



                var barcodeWriter = new BarcodeWriter
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Height = 150,
                        Width = 150
                    }
                };
                e1.Graphics.DrawImage(barcodeWriter.Write(codpedido), rectanguloQr);


            };
            try
            {

                p.PrintController = new StandardPrintController();
                p.Print();
            }
            catch (Exception ex)
            {
                throw new Exception("Problema en la impresión", ex);
            }
        }
        private void guardarMovVenta(int idpedido)
        {
            try
            {

                int totalapagar = getNumber(tb_total_a_pagar);

                mov_caja mov_caj = new mov_caja();
                mov_caj.idcaja = GlobalClass.idcaja.Value;
                mov_caj.idpedido = idpedido;
                mov_caj.total_ent = totalapagar;
                mov_caj.total_sal = 0;
                mov_caj.idtipomov = 1;

                int efec = getNumber(tb_efectivo);
                P.mov_caja.Add(mov_caj);
                var resul = P.SaveChanges();
                if (efec > 0)
                {
                    int vuelto = getNumber(tb_vuelto);
                    var caj = P.Caja.FirstOrDefault(x => x.codigo == GlobalClass.idcaja);
                    caj.efectivo_hay = caj.efectivo_hay - vuelto;

                    pagomov pagomov = new pagomov();
                    pagomov.mp_id = 1;
                    pagomov.total = efec;
                    pagomov.vuelto = vuelto;
                    pagomov.idmovcaja = mov_caj.idmovcaja;
                    P.pagomov.Add(pagomov);
                }
                int redcom = getNumber(tb_redcompra);
                if (redcom > 0)
                {
                    pagomov pagomov = new pagomov();
                    pagomov.mp_id = 2;
                    pagomov.total = redcom;
                    pagomov.vuelto = 0;
                    pagomov.idmovcaja = mov_caj.idmovcaja;
                    P.pagomov.Add(pagomov);
                }
                int transfe = getNumber(tb_transferencia);
                if (transfe > 0)
                {
                    pagomov pagomov = new pagomov();
                    pagomov.mp_id = 3;
                    pagomov.total = transfe;
                    pagomov.vuelto = 0;
                    pagomov.idmovcaja = mov_caj.idmovcaja;
                    P.pagomov.Add(pagomov);
                }
                int tarcre = getNumber(tb_credi);
                if (tarcre > 0)
                {
                    pagomov pagomov = new pagomov();
                    pagomov.mp_id = 4;
                    pagomov.total = tarcre;
                    pagomov.vuelto = 0;
                    pagomov.idmovcaja = mov_caj.idmovcaja;
                    P.pagomov.Add(pagomov);
                }
                int compraaqui = getNumber(tb_compraaqui);
                if (compraaqui > 0)
                {
                    pagomov pagomov = new pagomov();
                    pagomov.mp_id = 7;
                    pagomov.total = compraaqui;
                    pagomov.vuelto = 0;
                    pagomov.idmovcaja = mov_caj.idmovcaja;
                    P.pagomov.Add(pagomov);
                }
                int saldo_a_favor = getNumber(tb_saldo_favor);
                saldo_a_favor = saldo_a_favor - getNumber(tb_saldo_sobrante);
                if (saldo_a_favor > 0)
                {
                    pagomov pagomov = new pagomov();
                    pagomov.mp_id = 6;
                    pagomov.total = saldo_a_favor;
                    pagomov.vuelto = 0;
                    pagomov.idmovcaja = mov_caj.idmovcaja;
                    P.pagomov.Add(pagomov);
                }
                var resulcaj = P.SaveChanges();
            }
            catch (Exception)
            {
            }
        }
        private void getDespacho(int id)
        {

            List<ComandaDTO> comanda = controller.getComanda(id);

            StringBuilder textoComanda = new StringBuilder();
            string tot = new string(' ', 20);
            string str = new string(' ', 20);
            string com = new string(' ', 2);
            string space = new string(' ', 8);
            string spac = new string(' ', 4);
            string linea = new string('_', 25);

            textoComanda.AppendLine(str);
            if (comanda[0].despacho == "des")
            {
                textoComanda.AppendLine(str + "GUIA DE DESPACHO" + str);
                textoComanda.AppendLine(str);
            }
            if (comanda[0].despacho == "ret48")
            {
                textoComanda.AppendLine(str + "RETIRO 48HH" + str);
                textoComanda.AppendLine(str);
            }

            textoComanda.AppendLine("N° PEDIDO: " + comanda[0].id);
            textoComanda.AppendLine("FECHA Y HORA: " + comanda[0].fecha);
            textoComanda.AppendLine("VENDEDOR: " + comanda[0].vendedor);
            textoComanda.AppendLine(linea);
            if (comanda[0].despacho == "des")
            {
                textoComanda.AppendLine(str + "CLIENTE" + str);
                textoComanda.AppendLine(str);
                textoComanda.AppendLine("NOMBRE: " + comanda[0].cliente);
                textoComanda.AppendLine("TELEFONO: " + comanda[0].telefono);
                textoComanda.AppendLine("DIRECCIÓN: " + comanda[0].Direccion);
                textoComanda.AppendLine("FLETE: " + comanda[0].flete);
                textoComanda.AppendLine(linea);
            }
            if (comanda[0].despacho == "ret48")
            {
                textoComanda.AppendLine("RETIRA EN UN MAXIMO 48 HORAS: ");
            }
            textoComanda.AppendLine("CANT" + com + "PRODUCTO" + com + "CEN");
            textoComanda.AppendLine(str);
            int total = comanda[0].total;
            foreach (var item in comanda)
            {
                textoComanda.AppendLine("CODIGO PROD[" + item.cod_pro + "]");
                textoComanda.AppendLine(item.cantidad + space + item.producto + "[" + item.centro + "]");
            }
            textoComanda.AppendLine(linea);
            if (comanda[0].descuento != null)
            {
                textoComanda.AppendLine(str);
                textoComanda.AppendLine("TOTAL DESCUENTOS $ " + comanda[0].descuento);
            }
            textoComanda.AppendLine(tot + "TOTAL $ " + total + tot);
            textoComanda.AppendLine(linea);
            textoComanda.AppendLine("LOS PEDIDOS SON HASTA DONDE LLEGA EL VEHICULO");

            PrintDocument p = new PrintDocument();
            p.PrintPage += delegate (object sender1, PrintPageEventArgs e1)
            {
                e1.Graphics.DrawString(textoComanda.ToString(), new Font("Arial Bold", 11), new SolidBrush(System.Drawing.Color.Black), new RectangleF(0, 0, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height));

            };
            try
            {
                p.PrintController = new StandardPrintController();
                p.Print();
            }
            catch (Exception ex)
            {
                throw new Exception("Problema en la impresión", ex);
            }
        }
        private bool actualizarStock(int idproducto, int? idinventario, int cantidad)
        {
            inventario inv = new inventario();
            operacion ope = new operacion();
            if (idinventario != null)
            {
                inv = P.inventario.FirstOrDefault(x => x.idinventario == idinventario);
                if (inv != null)
                {
                    ope.idproducto = idproducto;
                    ope.stock_ant = inv.stock_total;
                    inv.stock_total = inv.stock_total - cantidad;
                    ope.stock_act = inv.stock_total;
                }

                if (inv == null)
                {
                    return false;
                }
                else
                {
                    ope.idtipo_operacion = 1;
                    ope.idinventario = inv.idinventario;
                    ope.fecha_hora = DateTime.Now;
                    ope.cantidad = cantidad;

                    P.operacion.Add(ope);
                    return true;
                }
            }
            else
            {
                return false;
            }


        }
        public string GetBoleta()
        {
            StringBuilder textoComanda = new StringBuilder();
            List<ComandaDTO> comanda = controller.getComanda(GlobalClass.idpedido);

            string tot = new string(' ', 20);
            string str = new string(' ', 10);
            string com = new string(' ', 2);
            string space = new string(' ', 8);
            string spac = new string(' ', 4);
            string linea = new string('_', 25);

            textoComanda.AppendLine(str + "COMPROBANTE VENTA" + str);
            textoComanda.AppendLine(str);

            textoComanda.AppendLine("N° PEDIDO: " + comanda[0].id);
            textoComanda.AppendLine("VENDEDOR: " + comanda[0].vendedor);
            textoComanda.AppendLine("FECHA Y HORA: " + comanda[0].fecha);
            textoComanda.AppendLine(linea);
            textoComanda.AppendLine("CANT" + com + "PRODUCTO");
            textoComanda.AppendLine(str);
            int total = comanda[0].total;
            foreach (var item in comanda)
            {
                textoComanda.AppendLine("CODIGO PROD[" + item.cod_pro + "]");
                textoComanda.AppendLine("  " + item.cantidad + space + item.producto + space + "$" + item.precio);
            }
            textoComanda.AppendLine(linea);
            if (comanda[0].descuento != null)
            {
                textoComanda.AppendLine(str);
                textoComanda.AppendLine("TOTAL DESCUENTOS $ " + comanda[0].descuento);
            }
            textoComanda.AppendLine(tot + "TOTAL $ " + total + tot);
            textoComanda.AppendLine(linea);

            return textoComanda.ToString();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GlobalClass.productos.Clear();
            this.Close();
        }
        private int getNumber(object text)
        {
            string valor = "";
            var type = text.GetType().ToString();
            if (type.Contains("Label"))
            {
                if ((text as Label).Content != null)
                {
                    valor = (text as Label).Content.ToString().Replace(".", "").ToString();
                }
            }
            else
            {
                valor = (text as TextBox).Text.ToString().Replace(".", "").ToString();
            }

            int num = 0;
            if (int.TryParse(valor, out num))
            {
                return num;
            }
            return num;
        }
        private void formatoInputNumber(Label textBox)
        {
            textBox.Content = String.Format("{0:N0}", getNumber(textBox));
        }
        private void calcularPago()
        {
            int descuen = descuento;
            int total = GlobalClass.total;
            int efec = getNumber(tb_efectivo);
            int redcom = getNumber(tb_redcompra);
            int transfe = getNumber(tb_transferencia);
            int tarcre = getNumber(tb_credi);
            int compraaqui = getNumber(tb_compraaqui);

            if (descuen > 0)
            {

                int desc_aplicado = (total * descuen / 100);
                total = total - desc_aplicado;


                int? total_desc = GlobalClass.productos.Sum(x => x.precio_desc);
                total_desc += desc_aplicado;
                if (total_desc != null)
                {
                    tb_descuento.Content = String.Format("{0:N0}", total_desc);
                }
                total_descuento = desc_aplicado;
            }
            else
            {
                total_descuento = 0;
                int? total_desc = GlobalClass.productos.Sum(x => x.precio_desc);
                if (total_desc != null)
                {
                    tb_descuento.Content = String.Format("{0:N0}", total_desc);
                }
            }


            int saldo_favor = 0;
            if (GlobalClass.saldo_devolucion != null)
            {
                saldo_favor = GlobalClass.saldo_devolucion.Value;
            }

            int totalpagado = efec + redcom + transfe + tarcre + compraaqui + saldo_favor;
            int totalapagar = total;

            int porpagar = totalapagar - totalpagado;
            if (porpagar <= 0)
            {
                tb_porpagar.Content = "0";
            }
            else
            {
                tb_porpagar.Content = porpagar.ToString();
            }

            tb_total_pagado.Content = totalpagado.ToString();
            int vuelto = totalpagado - totalapagar;
            if (vuelto >= 0)
            {
                tb_vuelto.Content = vuelto.ToString();
            }
            else
            {
                tb_vuelto.Content = "0";
            }
            tb_total_a_pagar.Content = totalapagar.ToString();

            int sobrante = saldo_favor - totalapagar;
            if (sobrante <= 0)
            {
                tb_saldo_sobrante.Content = "0";
            }
            else
            {
                tb_saldo_sobrante.Content = sobrante.ToString(); ;
            }

            formatoInputNumber(tb_vuelto);
            formatoInputNumber(tb_total_a_pagar);
            formatoInputNumber(tb_total_pagado);
            formatoInputNumber(tb_porpagar);
            formatoInputNumber(tb_saldo_sobrante);
        }
        private void tb_efectivo_KeyUp(object sender, KeyEventArgs e)
        {
            calcularPago();
        }

        private void tb_redcompra_KeyUp(object sender, KeyEventArgs e)
        {
            calcularPago();
        }

        private void tb_transferencia_KeyUp(object sender, KeyEventArgs e)
        {
            calcularPago();
        }

        private void tb_debito_KeyUp(object sender, KeyEventArgs e)
        {
            calcularPago();
        }

        private void tb_compraaqui_KeyUp(object sender, KeyEventArgs e)
        {
            calcularPago();
        }

        private void check_efectivo_Checked(object sender, RoutedEventArgs e)
        {
            tb_transferencia.Text = "0";
            tb_redcompra.Text = "0";
            tb_credi.Text = "0";
            tb_compraaqui.Text = "0";

            tb_efectivo.Text = getNumber(tb_total_a_pagar).ToString();
            calcularPago();
        }

        private void check_transferencia_Checked(object sender, RoutedEventArgs e)
        {
            tb_efectivo.Text = "0";
            tb_vuelto.Content = "0";
            tb_redcompra.Text = "0";
            tb_credi.Text = "0";
            tb_compraaqui.Text = "0";

            tb_transferencia.Text = getNumber(tb_total_a_pagar).ToString();
            calcularPago();
        }

        private void check_redcompra_Checked(object sender, RoutedEventArgs e)
        {
            tb_efectivo.Text = "0";
            tb_vuelto.Content = "0";
            tb_transferencia.Text = "0";
            tb_credi.Text = "0";
            tb_compraaqui.Text = "0";

            tb_redcompra.Text = getNumber(tb_total_a_pagar).ToString();
            calcularPago();
        }

        private void check_credito_Checked(object sender, RoutedEventArgs e)
        {
            tb_efectivo.Text = "0";
            tb_vuelto.Content = "0";
            tb_transferencia.Text = "0";
            tb_redcompra.Text = "0";
            tb_compraaqui.Text = "0";

            tb_credi.Text = getNumber(tb_total_a_pagar).ToString();
            calcularPago();
        }

        private void check_compraaqui_Checked(object sender, RoutedEventArgs e)
        {
            tb_efectivo.Text = "0";
            tb_vuelto.Content = "0";
            tb_transferencia.Text = "0";
            tb_redcompra.Text = "0";
            tb_credi.Text = "0";

            tb_compraaqui.Text = getNumber(tb_total_a_pagar).ToString();
            calcularPago();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }


        /*DEVOLUCION*/
        private void devolucion()
        {
            guardarMov();
            cambiarEstadoDevolucion();
            string ticket = GetTicketDevolucion();
            PrintDocument p = new PrintDocument();
            p.PrintPage += delegate (object sender1, PrintPageEventArgs e1)
            {
                e1.Graphics.DrawString(ticket, new Font("Arial Bold", 9), new SolidBrush(System.Drawing.Color.Black), new RectangleF(0, 0, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height));

            };
            try
            {
                p.PrintController = new StandardPrintController();
                p.Print();
            }
            catch (Exception ex)
            {
                throw new Exception("Problema en la impresión", ex);
            }
        }
        private void cambiarEstadoDevolucion()
        {
            venta_devolucion venta_Devolucion = P.venta_devolucion.FirstOrDefault(x => x.idventadevolucion == GlobalClass.idventadevolucion);
            venta_Devolucion.idestadodevolucion = 2;
            P.SaveChanges();

        }
        private void guardarMov()
        {
            int totalapagar = getNumber(tb_saldo_sobrante);

            mov_caja mov_caj = new mov_caja();
            mov_caj.idcaja = GlobalClass.idcaja.Value;
            mov_caj.idventadevolucion = GlobalClass.idventadevolucion;
            mov_caj.total_ent = 0;
            mov_caj.total_sal = totalapagar;
            mov_caj.idtipomov = 2;


            int efec = totalapagar;
            P.mov_caja.Add(mov_caj);
            var resul = P.SaveChanges();
            if (efec > 0)
            {
                int vuelto = 0;
                var caj = P.Caja.FirstOrDefault(x => x.codigo == GlobalClass.idcaja);
                caj.efectivo_hay = caj.efectivo_hay - vuelto;

                pagomov pagomov = new pagomov();
                pagomov.mp_id = 1;
                pagomov.total = efec;
                pagomov.vuelto = vuelto;
                pagomov.idmovcaja = mov_caj.idmovcaja;
                P.pagomov.Add(pagomov);
            }


            var resulcaj = P.SaveChanges();
        }

        private Bitmap GetBarcode(string codigo)
        {
            try
            {
                BarcodeWriter barcodeWriter = new BarcodeWriter();
                barcodeWriter.Format = BarcodeFormat.QR_CODE;
                barcodeWriter.Options.Height = 50;
                barcodeWriter.Options.Width = 100;
                return barcodeWriter.Write(codigo);
            }
            catch (Exception)
            {
                return null;
            }

        }
        public string GetTicketDevolucion()
        {
            StringBuilder textoComanda = new StringBuilder();

            string tot = new string(' ', 20);
            string str = new string(' ', 10);
            string com = new string(' ', 2);
            string space = new string(' ', 8);
            string spac = new string(' ', 4);
            string linea = new string('_', 25);

            textoComanda.AppendLine(str + "FERRETERIA SAN ALFONSO" + str);
            textoComanda.AppendLine(str + "TICKET DEVOLUCIÓN" + str);
            textoComanda.AppendLine(str);

            textoComanda.AppendLine("N° : " + GlobalClass.idventadevolucion);
            textoComanda.AppendLine("FECHA Y HORA: " + DateTime.Now.Date);
            textoComanda.AppendLine(linea);
            textoComanda.AppendLine(tot + "TOTAL: $ -" + getNumber(tb_saldo_sobrante) + tot);
            textoComanda.AppendLine(linea);
            textoComanda.AppendLine(str + "GRACIAS POR LA VISITA" + str);
            textoComanda.AppendLine(str);

            return textoComanda.ToString();
        }
        private bool? msj(string msj, bool? canc = null, bool? time = null)
        {
            ModalMensaje m = new ModalMensaje(msj, canc, time);
            return m.ShowDialog();
        }

        private void check_factura_Checked(object sender, RoutedEventArgs e)
        {
            if (check_factura.IsChecked == true)
            {
                btn_factura.Visibility = Visibility.Visible;
            }
            else
            {
                btn_factura.Visibility = Visibility.Hidden;
            }

        }

        private void btn_factura_Click(object sender, RoutedEventArgs e)
        {
            ModalFactura modalFactura = new ModalFactura();
            var r = modalFactura.ShowDialog();

            if (r == true)
            {
                ModalMensaje modal = new ModalMensaje("Factura modificada");
                modal.ShowDialog();
            }
        }

        private void check_factura_Unchecked(object sender, RoutedEventArgs e)
        {
            if (check_factura.IsChecked == true)
            {
                btn_factura.Visibility = Visibility.Visible;
            }
            else
            {
                btn_factura.Visibility = Visibility.Hidden;
            }

        }

        private void btn_descuento_Click(object sender, RoutedEventArgs e)
        {
            if (btn_descuento.Content.ToString() == "DESCUENTO ACTIVADO")
            {
                btn_descuento.Content = "REALIZAR DESCUENTO";
                ModalMensaje msj = new ModalMensaje("DESCUENTO DESACTIVADO", false, true);
                msj.ShowDialog();

            }
            else
            {
                HabilitarProceso hb = new HabilitarProceso("Habilitar descuento", 1);
                bool? hab = hb.ShowDialog();
                if (hab == true)
                {

                    btn_descuento.Content = "DESCUENTO ACTIVADO";
                    ModalMensaje msj = new ModalMensaje("DESCUENTO ACTIVADO", false, true);
                    msj.ShowDialog();

                    modal_descuento m = new modal_descuento();
                    m.ShowDialog();
                    descuento = m.porcetaje;
                    calcularPago();
                }
            }
        }
    }
}
