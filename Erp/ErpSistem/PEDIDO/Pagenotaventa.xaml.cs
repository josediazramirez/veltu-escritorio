using Controller;
using DTO;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ZXing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace ErpClass
{
    /// <summary>
    /// Lógica de interacción para Pagecliente.xaml
    /// </summary>
    public partial class Pagenotaventa : Page
    {
        InformeController controllers = new InformeController();
        public string name { get; set; }
        public string id { get; set; }
        ModelDataBase P = new ModelDataBase();
        ProductoController fn_pro = new ProductoController();
        ClienteController controllerCliente = new ClienteController();
        ObservableCollection<DetalleDTO> detalles = new ObservableCollection<DetalleDTO>();
        ObservableCollection<DetalleDTO> productos = new ObservableCollection<DetalleDTO>();
        int total;
        int porcetaje_desc = 0;
        public Pagenotaventa()
        {

            InitializeComponent();
            productos.Clear();
            mantenerDatos();
            cargarPedido();
            carCbxFlete();
            dgi_detalles.Columns[9].IsReadOnly = true;
        }

        private void cargarPedido()
        {
            dgi_detalles.ItemsSource = detalles;
        }
        private void carCbxFlete()
        {

            List<productoDTO> pro = new List<productoDTO>();
            pro.Add(new productoDTO() { codigo = 0, nombre = "- SELECCIONE -" });
            foreach (var item in fn_pro.getFlete())
            {
                pro.Add(new productoDTO() { codigo = item.codigo, nombre = item.precio.ToString(), precio = item.precio });
            }
            cbx_flete.ItemsSource = pro;
        }

        private string getText(string text)
        {
            if (text != null)
            {
                return text;
            }
            else
            {
                return "";
            }
        }
        private void btn_mas_Click(object sender, RoutedEventArgs e)
        {
            DetalleDTO detalle = (DetalleDTO)dgi_detalles.SelectedItem;
            if (detalle.Cantidad < detalle.cantidad_disp)
            {
                int desc = 0;
                if (detalle.precio_desc != null)
                {
                    desc = detalle.precio_desc.Value;
                }
                detalle.Cantidad++;
                detalle.total = detalle.cantidad * (detalle.subtotal - desc);
                total += detalle.subtotal - desc;
                mantenerDatos();
            }
            else
            {
                mensaje("El stock maximo es " + detalle.cantidad_disp);
            }
        }
        private void btn_menos_Click(object sender, RoutedEventArgs e)
        {
            DetalleDTO detalle = (DetalleDTO)dgi_detalles.SelectedItem;
            int cantidad = detalle.cantidad;
            if ((cantidad - 1) != 0)
            {
                int desc = 0;
                if (detalle.precio_desc != null)
                {
                    desc = detalle.precio_desc.Value;
                }

                detalle.Cantidad--;
                detalle.total = detalle.cantidad * (detalle.subtotal - desc);
                total -= detalle.subtotal - desc;

                mantenerDatos();
            }
        }
        private void btn_delete(object sender, RoutedEventArgs e)
        {
            DetalleDTO d = ((DetalleDTO)dgi_detalles.SelectedItem);
            total = total - d.total;
            mantenerDatos();
            var objeto = dgi_detalles.SelectedItem as DetalleDTO;
            detalles.Remove(objeto);
        }
        private string sintilde(string text)
        {
            return Regex.Replace(text.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "");
        }
        private void realizarComanda_Click(object sender, RoutedEventArgs e)
        {
            if (detalles.Count > 0)
            {
                var rest = msj("¿DESEA REALIZAR EL PEDIDO?", true);
                if (rest == true)
                {
                    int error = 0;
                    if (despacho.IsChecked == true)
                    {
                        {
                            int num = 0;
                            if (int.TryParse(tb_telefono.Text, out num) == false || tb_telefono.Text.Length != 9)
                            {
                                mensaje("Debe ingresar telefono 9 numeros");
                                error = 1;
                            }
                            else if (string.IsNullOrEmpty(tb_nombre.Text) == true)
                            {
                                mensaje("Debe ingresar nombre");
                                error = 1;
                            }
                            else if (string.IsNullOrEmpty(tb_direccion.Text) == true)
                            {
                                mensaje("Debe ingresar dirección");
                                error = 1;
                            }
                            else if (cbx_flete.SelectedIndex == 0)
                            {
                                mensaje("Debe seleccionar la tarifa del flete");
                                error = 1;
                            }
                            else
                            {
                                clienteDTO cli = new clienteDTO();
                                cli.nombre = tb_nombre.Text.ToLower();
                                cli.direccion = tb_direccion.Text.ToLower();
                                cli.id_telefono = int.Parse(tb_telefono.Text);
                                if (int.TryParse(tb_telefono_opc.Text, out num) == true && tb_telefono.Text.Length == 9)
                                {
                                    cli.telefono_opc = int.Parse(tb_telefono_opc.Text);
                                }
                                var clien = controllerCliente.guardarCliente(cli);
                                if (clien != null)
                                {
                                    cli.idcliente = clien.idcliente;
                                    GlobalClass.obj_cliente = cli;
                                }
                            }

                        }
                    }
                    if (error == 0)
                    {


                        Pedido pedido = new Pedido();
                        pedido.cli_nombre = GlobalClass.user_nombre;
                        if (retiro.IsChecked == true)
                        {
                            pedido.despacho = "ret48";
                        }
                        if (despacho.IsChecked == true)
                        {
                            pedido.idcliente = GlobalClass.obj_cliente.idcliente;
                            pedido.despacho = "des";
                        }
                        pedido.idestado = 1;
                        pedido.fecha = DateTime.Now;



                        pedido.precio_total = total;


                        if (despacho.IsChecked == true)
                        {
                            var flete = cbx_flete.SelectedItem as productoDTO;
                            pedido.idflete = flete.codigo;
                        }

                        P.Pedido.Add(pedido);
                        int v = 0;
                        v = P.SaveChanges();

                        if (v > 0)
                        {
                            int totalDes = 0;
                            foreach (DetalleDTO item in dgi_detalles.Items)
                            {
                                Detalle_pedido deta = new Detalle_pedido();
                                deta.cantidad = item.Cantidad;
                                deta.precio = item.precio;
                                deta.codigo = item.codigo;
                                deta.tamanio = item.Tamanio;
                                deta.precio_desc = item.Precio_desc;
                                if (deta.precio_desc != null)
                                {
                                    totalDes += item.Precio_desc.Value * item.Cantidad;
                                }
                                deta.idinventario = item.idinventario;
                                deta.pedido = pedido.codigo;
                                deta.total = item.total;
                                P.Detalle_pedido.Add(deta);
                            }
                            if (despacho.IsChecked == true)
                            {
                                var flete = cbx_flete.SelectedItem as productoDTO;
                                Detalle_pedido detalle_Pedido = new Detalle_pedido();

                                detalle_Pedido.cantidad = 1;
                                detalle_Pedido.precio = flete.precio;
                                detalle_Pedido.codigo = flete.codigo;
                                detalle_Pedido.pedido = pedido.codigo;
                                detalle_Pedido.total = flete.precio;
                                P.Detalle_pedido.Add(detalle_Pedido);
                            }

                            var r = P.SaveChanges();
                            var ate = controllers.GetUltimaAtencion();
                            atencion aten = new atencion();
                            if (ate != null)
                            {
                                aten.numero_atencion = ate.numero_atencion + 1;
                            }
                            else
                            {
                                aten.numero_atencion = 1;
                            }
                            aten.idpedido = pedido.codigo;
                            aten.idtipoatencion = 1;
                            aten.idestado_atencion = 1;
                            aten.fecha = DateTime.Now;
                            aten.vendedor = pedido.cli_nombre;
                            P.atencion.Add(aten);

                            int idatencion = P.SaveChanges();

                            printTicket(aten.numero_atencion);
                            detalles.Clear();
                            total = 0;
                            mantenerDatos();


                            try
                            {
                                GlobalClass.productos.Clear();
                                GlobalClass.estado = 1;
                                despacho.IsChecked = false;
                                tb_nombre.Clear();
                                tb_telefono.Clear();
                                tb_telefono_opc.Clear();
                                tb_direccion.Clear();
                                cbx_flete.SelectedIndex = 0;
                                dgi_producto.ItemsSource = null;
                                txt_buscar.Text = string.Empty;
                                ModalMensaje modal = new ModalMensaje("PEDIDO REALIZADO, IMPRIMIENDO TICKET", null, true);
                                modal.ShowDialog();
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Problema en la impresión", ex);
                            }
                        }
                        else
                        {
                            msj("Error en la base de datos");
                        }
                    }
                }
            }
            else
            {
                msj("DEBE INGRESAR PRODUCTOS");
            }

        }
        private void printTicket(int NroAtencion)
        {
            string comanda = GetBoleta(NroAtencion);


            string[] lines = comanda.ToString().Split(Environment.NewLine.ToCharArray());

            int lineas = (lines.Length * 20) + 600;

            PrintDocument p = new PrintDocument();
            PrinterSettings settings = new PrinterSettings();
            PaperSize paperSize = new PaperSize("Ticket", 300, lineas);
            settings.DefaultPageSettings.PaperSize = paperSize;

            p.PrinterSettings = settings;
            p.PrintPage += delegate (object sender1, PrintPageEventArgs e1)
            {
                Font font = new Font("Arial Bold", 24);
                e1.Graphics.DrawString(NroAtencion.ToString(), font, new SolidBrush(System.Drawing.Color.Black), 40, 10);

                System.Drawing.Rectangle rectanguloQr =
                new System.Drawing.Rectangle(
                    20, 50,
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
                e1.Graphics.DrawImage(barcodeWriter.Write(NroAtencion.ToString()), rectanguloQr);


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
        public string GetBoleta(int idpedido)
        {


            StringBuilder textoComanda = new StringBuilder();
            string str = new string(' ', 5);
            string str2 = new string(' ', 20);
            string linea = new string('_', 25);
            string com = new string(' ', 2);
            string space = new string(' ', 8);

            textoComanda.AppendLine(str2 + idpedido + str2);


            return textoComanda.ToString();
        }
        public string GetCotizacion(int idpedido, string vendedor)
        {


            StringBuilder textoComanda = new StringBuilder();
            string str = new string(' ', 5);
            string str2 = new string(' ', 20);
            string linea = new string('_', 25);
            string com = new string(' ', 2);
            string space = new string(' ', 8);

            //77.235.564-5
            //San alfonso spa
            //ferreteriasanalfonso@gmail.com
            //vicuña mackenna 941
            //966412350

            //textoComanda.AppendLine(str2 + "FERRETERIA" + str);
            //textoComanda.AppendLine(str2 + "SAN ALFONSO" + str);
            //textoComanda.AppendLine(linea);
            //textoComanda.AppendLine(str2 + "COTIZACIÓN" + str);
            textoComanda.AppendLine(linea);
            textoComanda.AppendLine("DATOS EMPRESA" + str);
            textoComanda.AppendLine("RAZÓN SOCIAL: SAN ALFONSO SPA" + str);
            textoComanda.AppendLine("RUT: 77.235.564-5" + str);
            textoComanda.AppendLine("CORREO: ferreteriasanalfonso@gmail.com" + str);
            textoComanda.AppendLine("DIRECCIÓN: vicuña mackenna 941" + str);
            textoComanda.AppendLine("TELEFONO: 966412350" + str);
            textoComanda.AppendLine(str);
            textoComanda.AppendLine(linea);
            textoComanda.AppendLine("INFO.CLIENTE");
            textoComanda.AppendLine(str);
            textoComanda.AppendLine("RUT: " + tb_rut.Text);
            textoComanda.AppendLine("NOMBRE: " + tb_nombre.Text);
            textoComanda.AppendLine("TELEFONO: " + tb_telefono.Text);
            textoComanda.AppendLine("DIRECCIÓN: " + tb_direccion.Text);
            textoComanda.AppendLine("CORREO: " + tb_correo.Text);
            textoComanda.AppendLine(linea);
            textoComanda.AppendLine(str);
            textoComanda.AppendLine("CANT" + com + "PRODUCTO" + com + "PRECIO");
            textoComanda.AppendLine(str);

            foreach (var item in detalles)
            {
                textoComanda.AppendLine("CODIGO PROD[" + item.codigo + "]");
                textoComanda.AppendLine(item.cantidad + space + item.producto + space + item.precio);

            }
            int flete_cost = 0;
            if (despacho.IsChecked == true)
            {
                var flete = cbx_flete.SelectedItem as productoDTO;
                textoComanda.AppendLine("FLETE" + space + flete.precio);
                flete_cost = flete.precio;
            }
            textoComanda.AppendLine(str);
            textoComanda.AppendLine("TOTAL");
            textoComanda.AppendLine(str);
            textoComanda.AppendLine(str2 + "$" + String.Format("{0:N0}", total + flete_cost) + str2);
            textoComanda.AppendLine(linea);
            textoComanda.AppendLine(str);
            textoComanda.AppendLine("FECHA Y HORA");
            textoComanda.AppendLine(str + DateTime.Now.ToString() + str);
            textoComanda.AppendLine(str);
            textoComanda.AppendLine("VENDEDOR");
            textoComanda.AppendLine(str + vendedor + str);
            textoComanda.AppendLine(str);
            textoComanda.AppendLine(linea);

            return textoComanda.ToString();
        }
        private void btn_pedido_Click(object sender, RoutedEventArgs e)
        {

            var producto = dgi_producto.SelectedItem as DetalleDTO;

            var prod = (from pro in P.Producto
                        join co in P.color
                        on pro.idcolor equals co.idcolor
                        into colors
                        from col in colors.DefaultIfEmpty()
                        join mar in P.marca
                       on pro.idmarca equals mar.idmarca
                       into mars
                        from marc in mars.DefaultIfEmpty()
                        join inv in P.inventario
                        on pro.codigo equals inv.idproducto
                        where pro.codigo == producto.codigo
                        && inv.idinventario == producto.idinventario
                        select new DetalleDTO
                        {
                            stock = inv.stock_total,
                            idmedida = inv.idmedida
                        }).FirstOrDefault();

            if (prod.stock == 0 || prod.stock == null)
            {
                msj("El producto no tiene stock");
            }
            else
            {
                if (prod.idmedida == 3)
                {
                    DetalleDTO detalle = new DetalleDTO();
                    detalle.codigo = producto.codigo;
                    detalle.producto = producto.producto;
                    detalle.idmedida = prod.idmedida;
                    detalle.Cantidad = 1;
                    detalle.cantidad_disp = (int)prod.stock.Value;

                    detalle.categoria = producto.categoria;
                    detalle.precio = producto.precio;
                    detalle.total = producto.precio;
                    detalle.subtotal = producto.precio;
                    detalle.idinventario = producto.idinventario;
                    detalle.centro = producto.centro;
                    detalle.autorizacion = producto.autorizacion;

                    AgregarDetalle(detalle);
                }
                else
                {
                    ModalCantidad modal = new ModalCantidad();
                    bool? res = modal.ShowDialog();
                    if (res == true)
                    {
                        int precio = modal.total;
                        int kg = 1000;
                        int costo = producto.precio;

                        int cant = (precio * kg) / producto.precio;
                        DetalleDTO detalle = new DetalleDTO();
                        detalle.codigo = producto.codigo;
                        detalle.producto = producto.producto;
                        detalle.idmedida = prod.idmedida;
                        detalle.Cantidad = cant;

                        detalle.categoria = producto.categoria;
                        detalle.precio = producto.precio;
                        detalle.total = precio;
                        detalle.subtotal = precio;
                        detalle.idinventario = producto.idinventario;
                        detalle.centro = producto.centro;
                        detalle.autorizacion = producto.autorizacion;

                        AgregarDetalle(detalle);
                    }
                }
            }
        }
        public void AgregarDetalle(DetalleDTO detalle)
        {
            var x = detalles.FirstOrDefault(ob => ob.idinventario == detalle.idinventario);

            if (detalle.autorizacion == 1)
            {
                HabilitarProceso hb = new HabilitarProceso("Autorizar Venta", 4);
                bool? hab = hb.ShowDialog();
                if (hab == true)
                {
                    if (x != null)
                    {
                        if (x.idmedida == 3)
                        {
                            if (x.Cantidad < detalle.cantidad_disp)
                            {
                                x.Cantidad++;
                                x.total = x.Cantidad * x.precio;
                                total += detalle.total;
                                mantenerDatos();
                            }
                            else
                            {
                                mensaje("El stock maximo es " + detalle.cantidad_disp);

                            }
                        }
                        else
                        {
                            detalles.Add(detalle);
                            total += detalle.total;
                            mantenerDatos();
                        }
                    }
                    else
                    {
                        detalles.Add(detalle);
                        total += detalle.total;
                        mantenerDatos();
                    }

                }
                else
                {
                    ModalMensaje modal = new ModalMensaje("No tiene acceso", null, true);
                    modal.ShowDialog();
                }
            }
            else
            {
                if (x != null)
                {
                    if (x.idmedida == 3)
                    {
                        if (x.Cantidad < detalle.cantidad_disp)
                        {
                            x.Cantidad++;
                            x.total = x.Cantidad * x.precio;
                            total += detalle.total;
                            mantenerDatos();
                        }
                        else
                        {
                            mensaje("El stock maximo es " + detalle.cantidad_disp);

                        }
                    }
                    else
                    {
                        detalles.Add(detalle);
                        total += detalle.total;
                        mantenerDatos();
                    }
                }
                else
                {
                    detalles.Add(detalle);
                    total += detalle.total;
                    mantenerDatos();
                }
            }

        }
        public void mantenerDatos()
        {
            String totalventa = String.Format("{0:N0}", total);
            lb_total.Content = "TOTAL $" + String.Format("{0:N0}", total);
        }

        private void trans_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void despacho_Click(object sender, RoutedEventArgs e)
        {

        }
        private void mensaje(string msj)
        {
            ModalMensaje modal = new ModalMensaje(msj);
            modal.ShowDialog();
        }

        private void txt_buscar_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var text = txt_buscar.Text.ToString().ToLower();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    string baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
                    var list = fn_pro.getFiltroProducto(text).Select(x => new DetalleDTO
                    {
                        codigo = x.codigo,
                        producto = x.nombre,
                        precio = x.precio,
                        ean = x.ean,
                        stock = x.stock_tot,
                        marca = x.marca,
                        color = x.color,
                        centro = x.centro,
                        idinventario = x.idinventario,
                        idmedida = x.idmedida,
                        autorizacion = x.autorizacion,
                        ImagenRuta = $"{baseUrl}/{x.imagen1}"
                    }).ToList();

                    if (list.Count > 0)
                    {
                        dgi_producto.ItemsSource = list;
                    }
                    else
                    {
                        msj("Producto no encontrado", null, true);
                    }
                }
                else
                {
                    dgi_producto.ItemsSource = null;
                }

            }
        }
        private bool? msj(string msj, bool? canc = null, bool? time = null)
        {
            ModalMensaje m = new ModalMensaje(msj, canc, time);
            return m.ShowDialog();
        }

        private void btn_descuento_Click(object sender, RoutedEventArgs e)
        {
            if (btn_descuento.Content.ToString() == "DESCUENTO ACTIVADO")
            {
                btn_descuento.Content = "REALIZAR DESCUENTO";
                dgi_detalles.Columns[9].IsReadOnly = true;
                ModalMensaje msj = new ModalMensaje("DESCUENTO DESACTIVADO", false, true);
                msj.ShowDialog();
            }
            else
            {
                HabilitarProceso hb = new HabilitarProceso("Habilitar descuento", 1);
                bool? hab = hb.ShowDialog();
                if (hab == true)
                {
                    dgi_detalles.Columns[9].IsReadOnly = false;
                    btn_descuento.Content = "DESCUENTO ACTIVADO";
                    ModalMensaje msj = new ModalMensaje("DESCUENTO ACTIVADO", false, true);
                    msj.ShowDialog();
                }
                else
                {
                    dgi_detalles.Columns[9].IsReadOnly = true;
                }
            }
        }

        private void dgi_detalles_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            calcularTotal();
        }
        private void calcularTotal()
        {
            int tot = 0;
            foreach (var item in detalles)
            {
                int desc = 0;
                if (item.Precio_desc != null)
                {
                    desc = item.Precio_desc.Value;
                }
                if (item.idmedida == 3)
                {
                    tot += (item.subtotal - desc) * item.Cantidad;
                }
                else
                {
                    tot += (item.subtotal - desc);
                }
            }
            total = tot;
            mantenerDatos();
        }

        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            ModalCantidad modal = new ModalCantidad();
            bool? res = modal.ShowDialog();
            if (res == true)
            {
                DetalleDTO detalle = (DetalleDTO)dgi_detalles.SelectedItem;
                int totalAnt = detalle.total;
                int precio = modal.total;
                int kg = 1000;
                int costo = detalle.precio;

                int cant = (precio * kg) / costo;

                int desc = 0;
                if (detalle.precio_desc != null)
                {
                    desc = detalle.precio_desc.Value;
                }
                detalle.Cantidad = cant;
                detalle.subtotal = precio;
                detalle.total = detalle.subtotal - desc;

                total -= totalAnt;
                total += detalle.subtotal - desc;
                mantenerDatos();
            }

        }

        private void btn_cotizacion_Click(object sender, RoutedEventArgs e)
        {
            if (detalles.Count > 0)
            {
                int error = 0;
                var rest = msj("¿DESEA REALIZAR LA COTIZACIÓN?", true);
                if (rest == true)
                {
                    int num = 0;
                    if (int.TryParse(tb_telefono.Text, out num) == false || tb_telefono.Text.Length != 9)
                    {
                        mensaje("Debe ingresar telefono 9 numeros");
                        error = 1;
                    }
                    else if (string.IsNullOrEmpty(tb_nombre.Text) == true)
                    {
                        mensaje("Debe ingresar nombre");
                        error = 1;
                    }
                    else
                    {
                        clienteDTO cli = new clienteDTO();
                        cli.nombre = tb_nombre.Text.ToLower();
                        cli.direccion = tb_direccion.Text.ToLower();
                        cli.id_telefono = int.Parse(tb_telefono.Text);
                        if (int.TryParse(tb_telefono_opc.Text, out num) == true && tb_telefono.Text.Length == 9)
                        {
                            cli.telefono_opc = int.Parse(tb_telefono_opc.Text);
                        }
                        var clien = controllerCliente.guardarCliente(cli);
                        if (clien != null)
                        {
                            cli.idcliente = clien.idcliente;
                            GlobalClass.obj_cliente = cli;
                        }
                    }
                    if (error == 0)
                    {
                        Pedido pedido = new Pedido();
                        pedido.cli_nombre = GlobalClass.user_nombre;
                        pedido.idestado = 4;
                        pedido.fecha = DateTime.Now;
                        pedido.precio_total = total;
                        pedido.idcliente = GlobalClass.obj_cliente.idcliente;
                        P.Pedido.Add(pedido);

                        int v = 0;
                        v = P.SaveChanges();

                        if (v > 0)
                        {
                            foreach (DetalleDTO item in dgi_detalles.Items)
                            {
                                Detalle_pedido deta = new Detalle_pedido();
                                deta.cantidad = item.Cantidad;
                                deta.precio = item.precio;
                                deta.codigo = item.codigo;
                                deta.tamanio = item.Tamanio;
                                deta.precio_desc = item.Precio_desc;
                                deta.idinventario = item.idinventario;
                                deta.pedido = pedido.codigo;
                                deta.total = item.total;
                                P.Detalle_pedido.Add(deta);
                            }
                            if (despacho.IsChecked == true)
                            {
                                var flete = cbx_flete.SelectedItem as productoDTO;
                                Detalle_pedido detalle_Pedido = new Detalle_pedido();

                                detalle_Pedido.cantidad = 1;
                                detalle_Pedido.precio = flete.precio;
                                detalle_Pedido.codigo = flete.codigo;
                                detalle_Pedido.pedido = pedido.codigo;
                                detalle_Pedido.total = flete.precio;
                                P.Detalle_pedido.Add(detalle_Pedido);
                            }

                            var r = P.SaveChanges();
                            var ate = controllers.GetUltimaAtencion();
                            atencion aten = new atencion();
                            if (ate != null)
                            {
                                aten.numero_atencion = ate.numero_atencion + 1;
                            }
                            else
                            {
                                aten.numero_atencion = 1;
                            }
                            aten.idpedido = pedido.codigo;
                            aten.idtipoatencion = 4;
                            aten.idestado_atencion = 2;
                            aten.fecha = DateTime.Now;
                            aten.vendedor = pedido.cli_nombre;
                            P.atencion.Add(aten);

                            int idatencion = P.SaveChanges();

                            string comanda = GetCotizacion(aten.numero_atencion, pedido.cli_nombre);
                            detalles.Clear();
                            total = 0;
                            mantenerDatos();

                            string[] lines = comanda.Split(Environment.NewLine.ToCharArray());

                            int lineas = (lines.Length * 20) + 600;

                            PrintDocument p = new PrintDocument();
                            PaperSize paperSize = new PaperSize("Ticket", 300, lineas);

                            p.DefaultPageSettings.PaperSize = paperSize;
                            p.PrintPage += delegate (object sender1, PrintPageEventArgs e1)
                            {
                                Graphics graphics = e1.Graphics;
                                Font drawFontBold = new Font("Arial", 10, System.Drawing.FontStyle.Regular, GraphicsUnit.Point);
                                Font drawFontTitulo = new Font("Arial", 12, System.Drawing.FontStyle.Bold, GraphicsUnit.Point);
                                SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.Black);


                                graphics.DrawString("FERRETERIA SAN ALFONSO", drawFontTitulo, drawBrush, new RectangleF(0, 10, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height));
                                graphics.DrawString("COTIZACIÓN", drawFontTitulo, drawBrush, new RectangleF(0, 50, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height));
                                graphics.DrawString(comanda, drawFontBold, drawBrush, new RectangleF(0, 70, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height));

                            };
                            try
                            {
                                p.PrintController = new StandardPrintController();
                                p.Print();
                                GlobalClass.productos.Clear();
                                GlobalClass.estado = 1;
                                despacho.IsChecked = false;
                                tb_nombre.Clear();
                                tb_telefono.Clear();
                                tb_telefono_opc.Clear();
                                tb_direccion.Clear();
                                cbx_flete.SelectedIndex = 0;
                                ModalMensaje modal = new ModalMensaje("COTIZACIÓN LISTA, IMPRIMIENDO TICKET", null, true);
                                modal.ShowDialog();
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Problema en la impresión", ex);
                            }
                        }
                        else
                        {
                            msj("Error en la base de datos");
                        }
                    }
                }
            }
            else
            {
                msj("DEBE INGRESAR PRODUCTOS");
            }
        }



        private void txt_cant_TextChanged(object sender, TextChangedEventArgs e)
        {
            DetalleDTO detalle = (DetalleDTO)dgi_detalles.SelectedItem;
            if (!string.IsNullOrEmpty((sender as TextBox).Text))
            {
                int cant = int.Parse((sender as TextBox).Text);

                if (cant <= detalle.cantidad_disp)
                {
                    int desc = 0;
                    if (detalle.precio_desc != null)
                    {
                        desc = detalle.precio_desc.Value;
                    }
                    total -= detalle.total;
                    detalle.Cantidad = cant;
                    detalle.total = detalle.Cantidad * (detalle.subtotal - desc);
                    total += detalle.total;
                    mantenerDatos();
                }
                else
                {
                    (sender as TextBox).Text = detalle.Cantidad.ToString();
                    mensaje("El stock maximo es " + detalle.cantidad_disp);
                }
            }

        }
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var img = sender as System.Windows.Controls.Image;
            if (img?.Source != null)
            {
                Window win = new Window
                {
                    Title = "Vista de Imagen",
                    Width = 600,
                    Height = 600,
                    Content = new System.Windows.Controls.Image
                    {
                        Source = img.Source,
                        Stretch = Stretch.Uniform
                    }
                };
                win.ShowDialog();
            }

        }

        private void btn_descuento_total_Click(object sender, RoutedEventArgs e)
        {
            if (btn_descuento.Content.ToString() == "DESCUENTO ACTIVADO")
            {
                btn_descuento.Content = "REALIZAR DESCUENTO";
                dgi_detalles.Columns[9].IsReadOnly = true;
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
                    porcetaje_desc = m.porcetaje;
                    calcularTotal();
                }
                else
                {
                    dgi_detalles.Columns[9].IsReadOnly = true;
                }
            }
        }
    }
}
