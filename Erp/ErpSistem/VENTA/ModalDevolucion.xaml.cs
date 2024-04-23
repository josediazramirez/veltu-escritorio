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

namespace ErpClass
{
    /// <summary>
    /// Lógica de interacción para MyModal.xaml
    /// </summary>
    public partial class ModalDevolucion : MetroWindow
    {
        ModelDataBase db = new ModelDataBase();
        private List<DetalleDTO> productos = new List<DetalleDTO>();
        private int codigo = 0;
        private int total = 0;
        public ModalDevolucion(List<DetalleDTO> productos, int total, int codigo)
        {

            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowStyle = WindowStyle.None;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.ShowCloseButton = false;
            this.productos = productos;
            this.codigo = codigo;
            this.total = total;
            lb_num_devolucion.Content = "N° " + codigo;
            lb_total.Content = "TOTAL $" + String.Format("{0:N0}", total);

        }
        private void btn_devolucion_Click(object sender, RoutedEventArgs e)
        {

            guardarMov();
            devolverProducto();
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
            this.DialogResult = true;
            this.Close();
        }
        private void cambiarEstadoDevolucion()
        {
            venta_devolucion venta_Devolucion = db.venta_devolucion.FirstOrDefault(x => x.idventadevolucion == codigo);
            venta_Devolucion.idestadodevolucion = 2;
            db.SaveChanges();

        }
        private void devolverProducto()
        {
            foreach (var item in productos)
            {
                actualizarStockDevolucion(item.codigo, item.idinventario, item.Cantidad);
            }
        }
        private bool actualizarStockDevolucion(int idproducto, int? idinventario, int cantidad)
        {

            inventario inv = new inventario();
            operacion ope = new operacion();

            inv = db.inventario.FirstOrDefault(x => x.idproducto == idproducto);
            if (inv != null)
            {
                ope.idproducto = idproducto;
                ope.stock_ant = inv.stock_total;
                inv.stock_total = inv.stock_total + cantidad;
                ope.stock_act = inv.stock_total;
            }

            if (inv == null)
            {
                return false;
            }
            else
            {
                ope.idtipo_operacion = 4;
                ope.idinventario = inv.idinventario;
                ope.fecha_hora = DateTime.Now;
                ope.cantidad = cantidad;

                db.operacion.Add(ope);
                return true;
            }


        }
        private void guardarMov()
        {
            int totalapagar = total;

            mov_caja mov_caj = new mov_caja();
            mov_caj.idcaja = GlobalClass.idcaja.Value;
            mov_caj.idventadevolucion = codigo;
            mov_caj.total_ent = 0;
            mov_caj.total_sal = totalapagar;
            mov_caj.idtipomov = 2;


            int efec = total;
            db.mov_caja.Add(mov_caj);
            var resul = db.SaveChanges();
            if (efec > 0)
            {
                int vuelto = 0;
                var caj = db.Caja.FirstOrDefault(x => x.codigo == GlobalClass.idcaja);
                caj.efectivo_hay = caj.efectivo_hay - vuelto;

                pagomov pagomov = new pagomov();
                pagomov.mp_id = 1;
                pagomov.total = efec;
                pagomov.vuelto = vuelto;
                pagomov.idmovcaja = mov_caj.idmovcaja;
                db.pagomov.Add(pagomov);
            }


            var resulcaj = db.SaveChanges();
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

            textoComanda.AppendLine("N° : " + codigo);
            textoComanda.AppendLine("FECHA Y HORA: " + DateTime.Now.Date);
            textoComanda.AppendLine(linea);
            textoComanda.AppendLine("CANT" + com + "PRODUCTO" + com + "PRECIO");
            textoComanda.AppendLine(str);

            foreach (var item in productos)
            {
                textoComanda.AppendLine("  -" + item.Cantidad + space + item.producto + space + "$" + item.precio);
            }
            textoComanda.AppendLine(linea);
            textoComanda.AppendLine(tot + "TOTAL: $ -" + total + tot);
            textoComanda.AppendLine(linea);
            textoComanda.AppendLine(str + "GRACIAS POR LA VISITA" + str);
            textoComanda.AppendLine(str);

            return textoComanda.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
