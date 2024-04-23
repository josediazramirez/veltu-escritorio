using Controller;
using DTO;
using MahApps.Metro.Controls;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace ErpClass
{
    /// <summary>
    /// Lógica de interacción para ModalPc.xaml
    /// </summary>
    public partial class ModalCamVen : MetroWindow
    {
        public string name { get; set; }
        public string id { get; set; }
        ModelDataBase P = new ModelDataBase();
        ProductoController controller = new ProductoController();
        ClienteController controllerCliente = new ClienteController();
        public static ObservableCollection<DetalleDTO> detalles = new ObservableCollection<DetalleDTO>();
        public static ObservableCollection<DetalleDTO> productos = new ObservableCollection<DetalleDTO>();
        public static int total = 0;
        public static int total_devo = 0;
        public static int total_diferencia=0;
        public static int caja_estado = 0;
        public ModalCamVen()
        {
            InitializeComponent();
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.ResizeMode = ResizeMode.CanResize;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.WindowState = WindowState.Maximized;
          
            this.ShowCloseButton = false;
            productos.Clear();
            cargarPedido();
            txt_buscar_codigo.Focus();
            carCbxFlete();
            total_devo = 0;
            total_devo = GlobalClass.saldo_devolucion.Value;
            lb_saldo_devo.Content = "SALDO DEVOLUCIÓN $ " + String.Format("{0:N0}", total_devo);
            total_diferencia = 0;
            lb_saldo_diferencia.Content = "SALDO DIFERENCIA $ " + String.Format("{0:N0}", total_diferencia);
            total = 0;
            mantenerDatos();
        }

        private void cargarPedido()
        {
            detalles.Clear();
            dgi_detalles.ItemsSource = detalles;
        }
        private void carCbxFlete()
        {

            List<productoDTO> pro = new List<productoDTO>();
            pro.Add(new productoDTO() { codigo = 0, nombre = "- SELECCIONE -" });
            foreach (var item in controller.getFlete())
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
            detalle.total = detalle.total + detalle.subtotal;
            total = total + detalle.subtotal;
            detalle.Cantidad++;
            int TOTAL_DEVO = total_devo;
            total_diferencia = TOTAL_DEVO - total;
            mantenerDatos();
        }
        private void btn_menos_Click(object sender, RoutedEventArgs e)
        {
            DetalleDTO detalle = (DetalleDTO)dgi_detalles.SelectedItem;
            int cantidad = detalle.Cantidad;
            if ((cantidad - 1) != 0)
            {
                detalle.total = detalle.total - detalle.subtotal;
                total = total - detalle.subtotal;
                detalle.Cantidad--;
                
                int TOTAL_DEVO = total_devo;
                total_diferencia = TOTAL_DEVO - total;
                mantenerDatos();
            }
        }
        private void btn_delete(object sender, RoutedEventArgs e)
        {
            DetalleDTO d = ((DetalleDTO)dgi_detalles.SelectedItem);
            total = total - d.total;
            int TOTAL_DEVO = total_devo;
            total_diferencia = TOTAL_DEVO - total;
            mantenerDatos();
            var objeto = dgi_detalles.SelectedItem as DetalleDTO;
            detalles.Remove(objeto);
        }

        private void txt_buscar_code_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

                if (txt_buscar_codigo.Text != null)
                {
                    long num = 0;
                    if (long.TryParse(txt_buscar_codigo.Text, out num))
                    {
                        long cod = long.Parse(txt_buscar_codigo.Text);
                        txt_buscar_codigo.Text = "";


                        var obj = controller.getProductoXcodigo(cod).Select(x => new DetalleDTO
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
                            Cantidad = 1,
                            total = x.precio,
                            subtotal = x.precio
                        }).FirstOrDefault();

                        if (obj != null)
                        {
                            AgregarDetalle(obj);
                        }
                        else
                        {
                            mensaje("PRODUCTO NO ENCONTRADO");
                        }
                    }
                    else
                    {
                        mensaje("Debe ingresar el codigo");
                    }


                }
            }
        }
        private string sintilde(string text)
        {
            return Regex.Replace(text.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "");
        }
        private void realizarComanda_Click(object sender, RoutedEventArgs e)
        {
           if (detalles.Count > 0)
            {
                var rest = MessageBox.Show("¿DESEA REALIZAR EL PEDIDO?", "REALIZAR PEDIDO", MessageBoxButton.YesNo);
                if (rest == MessageBoxResult.Yes)
                {
                    int error = 0;
                    if (despacho.IsChecked == true )
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
                            foreach (DetalleDTO item in dgi_detalles.Items)
                            {
                                Detalle_pedido deta = new Detalle_pedido();
                                deta.cantidad = item.Cantidad;
                                deta.precio = item.precio;
                                deta.codigo = item.codigo;
                                deta.tamanio = item.Tamanio;
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
                            int pro = P.SaveChanges();
                            if (pro>0)
                            {
                                GlobalClass.idpedido = pedido.codigo;
                                this.DialogResult = true;
                                this.Close();
                            }

                        }
                        else
                        {
                            MessageBox.Show("Error en la base de datos");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("DEBE INGRESAR PRODUCTOS", "INFORMACIÓN");
            }

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
                            stock = inv.stock_total
                        }).FirstOrDefault();

            if (prod.stock == 0 || prod.stock == null)
            {
                MessageBox.Show("El producto no tiene stock");
            }
            else
            {
                DetalleDTO detalle = new DetalleDTO();
                detalle.codigo = producto.codigo;
                detalle.producto = producto.producto;
                detalle.Cantidad = 1;

                detalle.categoria = producto.categoria;
                detalle.precio = producto.precio;
                detalle.total = producto.precio;
                detalle.subtotal = producto.precio;
                detalle.idinventario = producto.idinventario;
                detalle.centro = producto.centro;

                AgregarDetalle(detalle);
            }

        }
        public void AgregarDetalle(DetalleDTO detalle)
        {
            var x = detalles.FirstOrDefault(ob => ob.codigo == detalle.codigo);
            if (x != null)
            {
                x.Cantidad++;
                x.total = x.Cantidad * x.precio;
            }
            else
            {
                detalles.Add(detalle);
            }

            total += detalle.total;
            int TOTAL_DEVO = total_devo;
            total_diferencia = TOTAL_DEVO - total;
            mantenerDatos();

        }
        public void mantenerDatos()
        {
            String totalventa = String.Format("{0:N0}", total);
            lb_total.Content = "TOTAL $" + String.Format("{0:N0}", total);
            lb_saldo_devo.Content = "TOTAL DEVOLUCIÓN $" + String.Format("{0:N0}", total_devo);
            if (total_diferencia<=0)
            {
                
                lb_saldo_diferencia.Content = "DIFERENCIA FALTANTE $" + String.Format("{0:N0}", Math.Abs(total_diferencia));
            }
            else
            {
                lb_saldo_diferencia.Content = "DIFERENCIA SOBRANTE $" + String.Format("{0:N0}", total_diferencia);

            }

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

        private void btn_canl_cam_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        private void txt_buscar_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var text = txt_buscar.Text.ToString().ToLower();
                productos.Clear();
                var list = controller.getFiltroProducto(text).Select(x => new DetalleDTO
                {
                    codigo = x.codigo,
                    producto = x.nombre,
                    precio = x.precio,
                    ean = x.ean,
                    stock = x.stock_tot,
                    marca = x.marca,
                    color = x.color,
                    centro = x.centro,
                    idinventario = x.idinventario
                }).ToList();

                foreach (var item in list)
                {
                    productos.Add(item);
                }
                dgi_producto.ItemsSource = list;
            }

        }
    }
}
