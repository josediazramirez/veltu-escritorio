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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Controller;
using System.Data;
using DTO;
using System.Data.Entity.Core;
using System.Drawing.Printing;
using System.Drawing;
using Model;
using System.Windows.Threading;
using System.Data.Entity;
using ZXing;

namespace ErpClass
{
    /// <summary>
    /// Lógica de interacción para pagecomanda.xaml
    /// </summary>
    public partial class pageventadia : Page
    {
        InformeController controller = new InformeController();
        ModelDataBase model = new ModelDataBase();
        public pageventadia()
        {
            InitializeComponent();
            GridPedido(DateTime.Now.Date);

        }
        public async void GridPedido(DateTime fecha)
        {
            try
            {
                List<pedidoDTO> lista = new List<pedidoDTO>();
                lista = await controller.getPedidos(fecha);
                dg_comandas.ItemsSource = lista;
            }
            catch (EntityException e)
            {
                MessageBox.Show(e.Message);
            }

        }

        private void btn_detalle_Click(object sender, RoutedEventArgs e)
        {
            int id = ((pedidoDTO)dg_comandas.SelectedItem).codigo;

            List<ComandaDTO> comanda = controller.getComanda(id);

            StringBuilder textoComanda = new StringBuilder();
            string tot = new string(' ', 20);
            string str = new string(' ', 20);
            string com = new string(' ', 2);
            string space = new string(' ', 8);
            string spac = new string(' ', 4);
            string linea = new string('_', 25);

            //textoComanda.AppendLine(str + "FERRETERIA SAN ALFONSO" + str);
            //textoComanda.AppendLine(str);
            if (comanda[0].despacho == "des")
            {
                textoComanda.AppendLine(str + "GUIA DE DESPACHO" + str);
                textoComanda.AppendLine(str);
            }
            textoComanda.AppendLine("N° PEDIDO: " + comanda[0].id);
            textoComanda.AppendLine("FECHA Y HORA: " + comanda[0].fecha);
            textoComanda.AppendLine("VENDEDOR: " + comanda[0].vendedor);
            textoComanda.AppendLine(linea);
            if (comanda[0].despacho=="des")
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
                textoComanda.AppendLine("CANT" + com + "PRODUCTO"+com+ "CEN");
            textoComanda.AppendLine(str);
            int total = comanda[0].total;
            foreach (var item in comanda)
            {
                textoComanda.AppendLine(item.cantidad + space + item.producto + space + "[" + item.centro+"]");
            }
            textoComanda.AppendLine(linea);
            if (comanda[0].descuento != null)
            {
                textoComanda.AppendLine(str);
                textoComanda.AppendLine("TOTAL DESCUENTOS $ " + comanda[0].descuento);
            }
            textoComanda.AppendLine(tot + "TOTAL $ " + total + tot);
            textoComanda.AppendLine(linea);

            tb_comanda.Text = textoComanda.ToString();
            tb_comanda.Tag = id;
        }

        private void btn_imprimir_Click(object sender, RoutedEventArgs e)
        {

            if (tb_comanda.Text != string.Empty)
            {
                string comanda = tb_comanda.Text;
                string codpedido = tb_comanda.Tag.ToString();
                string[] lines = comanda.ToString().Split(Environment.NewLine.ToCharArray());

                int lineas = (lines.Length * 20) + 300;
                string textoFinal = "El PEDIDO ES HASTA DONDE VEHICULO PUEDA INGRESAR";
                PrintDocument p = new PrintDocument();
                PrinterSettings settings = new PrinterSettings();
                PaperSize paperSize = new PaperSize("Ticket",300, lineas);
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

                    Font font =new Font("Arial Bold", 11);
                    SizeF stringSize = new SizeF();
                    stringSize = e1.Graphics.MeasureString(comanda, font, e1.PageBounds.Width);

                    RectangleF rectanguloTexto= new RectangleF(0, rectanguloLogo.Bottom,
                        stringSize.Width,
                       stringSize.Height);

                    e1.Graphics.DrawString(comanda,
                        font, 
                        new SolidBrush(System.Drawing.Color.Black),
                        rectanguloTexto);

                    System.Drawing.Rectangle rectanguloQr =
                    new System.Drawing.Rectangle(
                        (e1.PageBounds.Width - 150) / 2, 
                        (int)rectanguloTexto.Bottom,
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
                   
                    e1.Graphics.DrawString(textoFinal,
                        new Font("Arial",7,System.Drawing.FontStyle.Bold,GraphicsUnit.Point),
                        new SolidBrush(System.Drawing.Color.Black),
                        0,rectanguloQr.Bottom);

                };
                try
                {
                    //System.Windows.Forms.PrintPreviewDialog printPreviewDialog = new System.Windows.Forms.PrintPreviewDialog();
                    //printPreviewDialog.Document = p;
                    //printPreviewDialog.ShowDialog();

                    p.PrintController = new StandardPrintController();
                    p.Print();
                }
                catch (Exception ex)
                {
                    throw new Exception("Problema en la impresión", ex);
                }
            }

        }

        private async void btn_cancelar_pedido_Click(object sender, RoutedEventArgs e)
        {
            int id = ((pedidoDTO)dg_comandas.SelectedItem).codigo;
            MessageBoxResult result = MessageBox.Show("¿Desea cancelar el pedido?", "Cancelación de pedido", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                await cancelar_pedido(id);
            }

        }
        public async Task cancelar_pedido(int id)
        {
            Pedido pedido = model.Pedido.FirstOrDefault(x => x.codigo == id);
            pedido.idestado = 3;
            model.Entry(pedido).State = EntityState.Modified;
            var listPedido = pedido.Detalle_pedido.ToList();
            List<agregado> agregados = new List<agregado>();
            foreach (var item in listPedido)
            {
                var ing = item.ExtraIngrediente.ToList();

                foreach (var i in ing)
                {
                    var cantidad = model.agregado.FirstOrDefault(x => x.idagregado == i.id_agregado).cantidad;

                    agregados.Add(new agregado() { idingre = i.id_ingre.Value, cantidad = cantidad });
                }
            }
            var listAgre = agregados.GroupBy(x => x.idingre);
            foreach (var item in listAgre)
            {
                var idingrediente = item.Key;
                var cantidad = 0;
                foreach (var itemIng in item)
                {
                    cantidad += itemIng.cantidad;
                }
                actualizarStock(idingrediente, 6, cantidad);
            }

            await model.SaveChangesAsync();
            var p = model.Pedido.ToList();
            //GridPedido();
            string pcid = GlobalClass.IdComputador;
            Caja caja = model.Caja.OrderByDescending(x => x.codigo).Take(1).FirstOrDefault();
            List<estado> estados = model.estado.ToList();
            List<pedidoDTO> lista = new List<pedidoDTO>();

            var lista_pedidos = await Task.Run(() => (from x in model.Pedido
                                                      where x.idComputador == pcid && x.fecha >= caja.hora_inicio
                                                      orderby x.numero descending
                                                      select x).ToList());
            foreach (var ped in lista_pedidos)
            {
                ped.Cliente = model.Cliente.Any(c => c.idcliente == ped.idcliente) ? model.Cliente.Single(c => c.idcliente == ped.idcliente) : null;
                pedidoDTO pedi = new pedidoDTO();
                pedi.codigo = ped.codigo;
                pedi.numero = ped.numero.Value;
                pedi.fecha = ped.fecha;
                if (ped.Cliente != null)
                {
                    pedi.cl_numero = ped.Cliente.id_telefono;
                }
                pedi.direccion = ped.Cliente != null ? ped.Cliente.direccion : "Local";
                pedi.num_dire = ped.Cliente != null ? ped.Cliente.num_direccion : null;
                pedi.cl_nombre = ped.Cliente != null && (string.IsNullOrEmpty(ped.cli_nombre) || string.IsNullOrWhiteSpace(ped.cli_nombre)) ? ped.Cliente.nombre : ped.cli_nombre;
                pedi.total = ped.precio_total;
                estado estado = estados.FirstOrDefault(x => x.idestado == ped.idestado);

                pedi.estado = estado.nombre;
                pedi.estado_color = estado.color;
                lista.Add(pedi);
            }
            dg_comandas.ItemsSource = lista;
            if (caja != null)
            {
                int cant = lista.Where(x => x.fecha >= caja.hora_inicio).Count();
                lb_num.Content = "Total " + cant;

            }
        }
        private bool actualizarStock(int idproducto, int idcategoria, int cantidad)
        {
            inventario inv = new inventario();
            operacion ope = new operacion();
            if (idcategoria == 6)
            {
                inv = model.inventario.FirstOrDefault(x => x.idingrediente == idproducto);
                if (inv != null)
                {
                    ope.idingrediente = idproducto;
                    ope.stock_ant = inv.stock_total;
                    inv.stock_total = inv.stock_total + cantidad;
                    ope.stock_act = inv.stock_total;
                }
            }
            else
            {
                inv = model.inventario.FirstOrDefault(x => x.idproducto == idproducto);
                if (inv != null)
                {
                    ope.idproducto = idproducto;
                    ope.stock_ant = inv.stock_total;
                    inv.stock_total = inv.stock_total + cantidad;
                    ope.stock_act = inv.stock_total;
                }
            }
            if (inv == null)
            {
                return false;
            }
            else
            {
                ope.idtipo_operacion = 8;
                ope.idinventario = inv.idinventario;
                ope.fecha_hora = DateTime.Now;
                ope.cantidad = cantidad;

                model.operacion.Add(ope);
                return true;
            }

        }

        private void btn_buscar_Click(object sender, RoutedEventArgs e)
        {
            DateTime? x_fecha = fecha.SelectedDate;
            if (x_fecha!=null)
            {
                GridPedido(x_fecha.Value);
            }
            else
            {
                ModalMensaje mensaje = new ModalMensaje("Debe ingresar fecha");
                mensaje.ShowDialog();
            }
            
        }
    }
}
