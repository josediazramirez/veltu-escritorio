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
    public partial class pagefactura : Page
    {
        InformeController controller = new InformeController();
        ModelDataBase model = new ModelDataBase();
        public pagefactura()
        {
            InitializeComponent();
            GridFacturas("2");

        }
        public async void GridFacturas(string estado)
        {
            try
            {
                int est = int.Parse(estado);
                List<FacturaDTO> lista = new List<FacturaDTO>();
                lista = await model.facturas.Where(x=>x.estado== est || est==2).Select(x => new FacturaDTO
                {
                    idfactura = x.idfactura,
                    rut=x.rut,
                    correo =x.correo,
                    numero = x.numero,
                    fecha = x.fecha,
                    estado = x.estado,
                    idpedido = x.idpedido
                }).ToListAsync();

                tabla_factura.ItemsSource = lista;
            }
            catch (EntityException e)
            {
                MessageBox.Show(e.Message);
            }

        }

        private void btn_detalle_Click(object sender, RoutedEventArgs e)
        {
            int id = ((FacturaDTO)tabla_factura.SelectedItem).idpedido;

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

     
        private void btn_buscar_Click(object sender, RoutedEventArgs e)
        {
                ComboBoxItem selectedItem = (ComboBoxItem)cbx_estado.SelectedItem;
                GridFacturas(selectedItem.Tag.ToString());

            
        }

        private async void btn_estado_Click(object sender, RoutedEventArgs e)
        {
            var fac = ((FacturaDTO)tabla_factura.SelectedItem);
            string msj = "";
            int est = 0;
            if (fac.estado == 0)
            {
                est = 1;
                msj = "¿Cambiar estado PENDIETE A HECHO?";
            }
            else
            {
                est = 0;
                msj = "¿Cambiar estado HECHO A PENDIENTE?";
            }
            ModalMensaje ms = new ModalMensaje(msj, true, null);
            ms.ShowDialog();
            if (ms.DialogResult == true)
            {
                factura factura = await model.facturas.FirstOrDefaultAsync(x=>x.idfactura==fac.idfactura);
                if (factura != null)
                {
                    factura.estado = est;
                   await model.SaveChangesAsync();
                    ComboBoxItem selectedItem = (ComboBoxItem)cbx_estado.SelectedItem;

                        GridFacturas(selectedItem.Tag.ToString());
                    
                }
            }
        }
    }
}
