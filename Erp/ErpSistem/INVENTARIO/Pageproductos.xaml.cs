using Controller;
using DTO;
using Microsoft.Win32;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFAutoCompleteTextbox;

namespace ErpClass
{
    /// <summary>
    /// Lógica de interacción para Pageproductos.xaml
    /// </summary>
    public partial class Pageproductos : Page
    {
        ProductoController controller = new ProductoController();
        ModelDataBase model = new ModelDataBase();
        ObservableCollection<DetalleDTO> produc_transfer = new ObservableCollection<DetalleDTO>();
        public Pageproductos()
        {
            InitializeComponent();
            fecha_actual.Content = DateTime.Now.Date.ToString("dd/MM/yyyy");
            tabla_tranfer_pro.ItemsSource = produc_transfer;
            cargarCentro();

            CargarUnidadMedida();
            cargarComboCategoria();

            form_inv.Visibility = Visibility.Hidden;

            cargarColor();
            cargarMarca();
            cargarTipoOperacion();
            cargarBodega();
            if (GlobalClass.user_rol == 3)
            {
                tb_precio_costo.Visibility = Visibility.Visible;
                label_pre_costo.Visibility = Visibility.Visible;
            }
            else
            {
                tb_precio_costo.Visibility = Visibility.Hidden;
                label_pre_costo.Visibility = Visibility.Hidden;
            }

        }
        public void cargarTipoOperacion()
        {
            var lista = model.tipo_operacion.ToList();
            Item c = new Item();

            cbx_tipo_mov.Items.Add(c);
            foreach (var item in lista)
            {
                Item ca = new Item();
                ca.codigo = item.idtipo_operacion;
                ca.nombre = item.nombre;

                cbx_tipo_mov.Items.Add(ca);
            }
        }
        public void cargarBodega()
        {
            var lista = model.centro.ToList();
            Item c = new Item();

            cbx_bodega.Items.Add(c);
            foreach (var item in lista)
            {
                Item ca = new Item();
                ca.codigo = item.idcentro;
                ca.nombre = item.nombre;

                cbx_bodega.Items.Add(ca);
            }
        }
        private void cargarComboProducto(int? codigo)
        {
            var lista = controller.getProductoInv(codigo);
            productoDTO c = new productoDTO();
            c.nombre = "- SELECCIONE -";
            c.codigo = 0;
            cbx_pro_inv.Items.Clear();
            cbx_pro_inv.Items.Add(c);
            cbx_pro_inv.SelectedIndex = 0;
            foreach (var item in lista)
            {
                cbx_pro_inv.Items.Add(new productoDTO()
                {
                    codigo = item.codigo,
                    nombre = item.nombre + "-" + item.categoria
                });
            }
            var centros = controller.GetCentros();
            cbx_desde.ItemsSource = centros;
            cbx_hasta.ItemsSource = centros;



        }
        private void cargarCentro()
        {
            var lista = controller.getCentro();
            Item c = new Item();
            cbx_ubicacion.Items.Add(c);
            foreach (var item in lista)
            {
                cbx_ubicacion.Items.Add(item);
            }

        }


        public async void btn_guardar_pro(object sender, RoutedEventArgs e)
        {

            try
            {
                productoDTO producto = new productoDTO();
                producto.codigo = int.Parse(codigo.Text);
                producto.nombre = tb_nombre.Text.ToUpper();
                producto.descripcion = tb_descripcion.Text;
                producto.lote = tb_lote.Text;
                if (string.IsNullOrEmpty(tb_ean.Text) == false)
                {
                    producto.ean = tb_ean.Text;
                }




                producto.categoria = (int)cbx_categoria.SelectedValue;
                producto.precio = int.Parse(tb_precio.Text);
                if ((int)cbx_marca.SelectedValue != 0)
                {
                    producto.idmarca = (int)cbx_marca.SelectedValue;
                }
                if ((int)cbx_color.SelectedValue != 0)
                {
                    producto.idcolor = (int)cbx_color.SelectedValue;
                }
                if (string.IsNullOrWhiteSpace(tb_precio_costo.Text) == false)
                {
                    producto.precio_costo = int.Parse(tb_precio_costo.Text);
                }

                byte[] imagen = ConvertImageToBytes(PreviewImage.Source);
                if (await controller.EditProducto(producto,imagen) > 0)
                {

                    var text = tb_filtro_producto.Text.ToString().ToLower().Split(' ');


                    var list = controller.getGrillaProductos(null);

                    for (int i = 0; i < text.Length; i++)
                    {
                        text[i] = sintilde(text[i]);
                        list = list.Where(x => sintilde(x.marca).ToLower().Replace(" ", "").Contains(text[i])
                        || sintilde(x.nombre).ToLower().Replace(" ", "").Contains(text[i])
                        || sintilde(x.color).ToLower().Replace(" ", "").Contains(text[i])).ToList();

                    }

                    dgrid_productos.ItemsSource = list;
                    btn_guardar.Visibility = Visibility.Hidden;
                    btn_cancelar.Visibility = Visibility.Hidden;
                    btn_addCli.Visibility = Visibility.Visible;

                    CleanControles();
                    MessageBox.Show("Producto actualizado");


                }
                else
                {

                    var text = tb_nombre.Text.ToString().ToLower().Split(' ');


                    var list = controller.getGrillaProductos(null);

                    for (int i = 0; i < text.Length; i++)
                    {
                        text[i] = sintilde(text[i]);
                        list = list.Where(x => sintilde(x.marca).ToLower().Replace(" ", "").Contains(text[i])
                        || sintilde(x.nombre).ToLower().Replace(" ", "").Contains(text[i])
                        || sintilde(x.color).ToLower().Replace(" ", "").Contains(text[i])).ToList();

                    }

                    dgrid_productos.ItemsSource = list;
                    btn_guardar.Visibility = Visibility.Hidden;
                    btn_cancelar.Visibility = Visibility.Hidden;
                    btn_addCli.Visibility = Visibility.Visible;

                    CleanControles();
                    MessageBox.Show("Producto sin cambios");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
        public void CargarUnidadMedida()
        {
            var lista = controller.getUnidadMedida();
            Item c = new Item();
            //cbx_uni_medida.Items.Add(c);
            cbx_unimedida_inv.Items.Add(c);
            foreach (var item in lista)
            {
                Item ca = new Item();
                ca.codigo = item.codigo;
                ca.nombre = item.sigla;
                //cbx_uni_medida.Items.Add(ca);
                cbx_unimedida_inv.Items.Add(ca);
            }

        }
        private void getInv()
        {
            gridinv.ItemsSource = controller.geInventario();
        }
        private void getMovimiento()
        {
            gridmovimiento.ItemsSource = controller.getMovimiento();
        }
        public void cargarGrid()
        {

            var id = (int)cbx_categoria.SelectedValue;
            if (id != 0)
            {
                dgrid_productos.ItemsSource = controller.getGrillaProductos(id);
            }
            else
            {
                dgrid_productos.ItemsSource = controller.getGrillaProductos(null);
            }
        }
        public void cargarComboCategoria()
        {
            var lista = controller.getCategorias();
            Item c = new Item();
            cbx_categoria.Items.Add(c);
            cbx_categoria_inv.Items.Add(c);

            foreach (var item in lista)
            {
                var it = new Item();
                it.codigo = item.codigo;
                it.nombre = item.nombre;
                cbx_categoria.Items.Add(it);
                cbx_categoria_inv.Items.Add(it);
            }

        }
        private void btn_editar_Click(object sender, RoutedEventArgs e)
        {
            tb_nombre.Clear();
            tb_precio.Clear();
            tb_descripcion.Clear();
            tb_lote.Clear();
           
            validar_nombre.Content = "";
            validar_categoria.Content = "";
            validar_precio.Content = "";
            codigo.Clear();
            productoDTO producto = (productoDTO)dgrid_productos.SelectedItem;

            if (producto.ean != null)
            {
                tb_ean.Text = producto.ean.ToString();
            }
            codigo.Text = producto.codigo.ToString();
            tb_nombre.Text = producto.nombre;
            cbx_categoria.SelectedValue = producto.categoria;

            tb_precio.IsEnabled = true;
            tb_precio.Text = producto.precio.ToString();
            tb_descripcion.Text = producto.descripcion;
            tb_lote.Text = producto.lote;
            if (producto.idmarca != null)
            {
                cbx_marca.SelectedValue = producto.idmarca;
            }
            if (producto.idcolor != null)
            {
                cbx_color.SelectedValue = producto.idcolor;
            }
            if (producto.precio_costo != null)
            {
                tb_precio_costo.Text = producto.precio_costo.ToString();
            }
            btn_guardar.Visibility = Visibility.Visible;
            btn_cancelar.Visibility = Visibility.Visible;
            btn_addCli.Visibility = Visibility.Hidden;

        }
        public void CleanControles()
        {
            tb_nombre.Clear();
            tb_precio.Clear();
            tb_precio_costo.Clear();
            tb_descripcion.Clear();
            tb_lote.Clear();
            cbx_categoria.SelectedIndex = 0;
            validar_nombre.Content = "";
            validar_categoria.Content = "";
            validar_precio.Content = "";
            codigo.Clear();

            tb_ean.Clear();
            cbx_marca.SelectedIndex = 0;
            cbx_color.SelectedIndex = 0;
            PreviewImage.Source = null;
        }
        private void btn_eliminarr_Click(object sender, RoutedEventArgs e)
        {
            int id = ((productoDTO)dgrid_productos.SelectedItem).codigo;
            MessageBoxResult message = MessageBox.Show("¿Desea eliminar el producto?", "Mensaje de alerta", MessageBoxButton.YesNo);
            if (message == MessageBoxResult.Yes)
            {
                int result = controller.RemoveProducto(id);
                if (result == 2)
                {
                    FiltrarProducto();
                    CleanControles();
                    MessageBox.Show("Producto eliminado");
                }
                else if (result == 1)
                {
                    MessageBox.Show("Producto no eliminado");
                }
            }


        }
        private void btn_eliminar_Click(object sender, RoutedEventArgs e)
        {
            int id = ((productoDTO)dgrid_productos.SelectedItem).codigo;

            MessageBoxResult message = MessageBox.Show("¿Desea eliminar el producto?", "Mensaje de alerta", MessageBoxButton.YesNo);
            if (message == MessageBoxResult.Yes)
            {
                int result = controller.RemoveProducto(id);
                if (result == 2)
                {
                    cargarGrid();
                    MessageBox.Show("Producto eliminado");
                }
                else if (result == 1)
                {
                    MessageBox.Show("Producto utilizado");
                }
            }


        }
        private async void btn_addCli_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int num = 0;
                if (string.IsNullOrWhiteSpace(tb_nombre.Text) == true)
                {
                    validar_nombre.Content = "*";
                }
                else if (cbx_categoria.SelectedIndex == 0)
                {
                    validar_categoria.Content = "*";
                }
                else
                {
                    bool isNum = int.TryParse(tb_precio.Text, out num);
                    if (isNum == false)
                    {
                        validar_precio.Content = "*";
                    }
                    else
                    {
                        validar_precio.Content = "";
                        validar_nombre.Content = "";
                        validar_categoria.Content = "";
                        productoDTO producto = new productoDTO();
                        producto.nombre = tb_nombre.Text;
                        producto.categoria = (int)cbx_categoria.SelectedValue;


                        producto.precio = int.Parse(tb_precio.Text);
                        if (string.IsNullOrEmpty(tb_ean.Text) == false)
                        {
                            producto.ean = tb_ean.Text;
                        }
                        producto.idcolor = (int)cbx_color.SelectedValue;
                        producto.idmarca = (int)cbx_marca.SelectedValue;


                        if (string.IsNullOrWhiteSpace(tb_precio_costo.Text) == false)
                        {
                            producto.precio_costo = int.Parse(tb_precio_costo.Text);
                        }
                        byte[] imagen = ConvertImageToBytes(PreviewImage.Source);
                        if (await controller.guardarProducto(producto, imagen) == 0)
                        {
                            CleanControles();
                            var text = tb_nombre.Text.ToString().ToLower().Split(' ');


                            var list = controller.getGrillaProductos(null);

                            for (int i = 0; i < text.Length; i++)
                            {
                                text[i] = sintilde(text[i]);
                                list = list.Where(x => sintilde(x.marca).ToLower().Replace(" ", "").Contains(text[i])
                                || sintilde(x.nombre).ToLower().Replace(" ", "").Contains(text[i])
                                || sintilde(x.color).ToLower().Replace(" ", "").Contains(text[i])).ToList();

                            }

                            dgrid_productos.ItemsSource = list;
                            MessageBox.Show("Producto agregado");
                        }
                        else
                        {
                            MessageBox.Show("Producto existente");
                        }

                    }
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        private void btn_caja_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Pagecaja.xaml", UriKind.Relative));
        }


        private void cbx_categoria_inv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var id = ((Item)((ComboBox)sender).SelectedItem).codigo;
            cargarComboProducto(id);

        }
        //INVENTARIO
        private void btn_editar_inv_Click(object sender, RoutedEventArgs e)
        {
            this.CleanControlesInv();
            InventarioDTO inv = (InventarioDTO)gridinv.SelectedItem;

            tb_cod_inv.Text = inv.cod_inv.ToString();
            cbx_categoria_inv.SelectedValue = inv.idcategoria;

            cbx_pro_inv.SelectedValue = inv.cod_pro;
            cbx_ubicacion.SelectedValue = inv.idubicacion;
            cbx_unimedida_inv.SelectedValue = inv.iduni_medida;

            tb_stock_min.Text = inv.stock_minimo;
            tb_stock_total.Text = inv.stock_total;
            tb_stock_total.IsEnabled = false;


            btn_guardar_inv.Visibility = Visibility.Visible;
            btn_cancelar_inv.Visibility = Visibility.Visible;
            btn_save_inv.Visibility = Visibility.Hidden;

            form_ing_egre.Visibility = Visibility.Hidden;
            form_inv.Visibility = Visibility.Visible;
            form_stock.Visibility = Visibility.Hidden;

            cbx_categoria_inv.IsEnabled = false;
            cbx_pro_inv.IsEnabled = false;
        }
        private void btn_eliminar_inv_Click(object sender, RoutedEventArgs e)
        {
            int id = ((InventarioDTO)gridinv.SelectedItem).cod_inv;

            MessageBoxResult message = MessageBox.Show("¿Desea eliminar el producto de inventario?", "Mensaje de alerta", MessageBoxButton.YesNo);
            if (message == MessageBoxResult.Yes)
            {
                int result = controller.RemoveProductoInv(id);
                if (result == 2)
                {

                    var text = tb_filtro_inv.Text.ToString().ToLower().Split(' ');


                    var list = controller.geInventario();

                    for (int i = 0; i < text.Length; i++)
                    {
                        text[i] = sintilde(text[i]);
                        list = list.Where(x => sintilde(x.nombre.ToString()).ToLower().Replace(" ", "").Contains(text[i])).ToList();

                    }
                    gridinv.ItemsSource = list;
                    CleanControles();
                    MessageBox.Show("Producto eliminado de inventario");
                }
                else if (result == 1)
                {
                    MessageBox.Show("Producto no eliminado no eliminado");
                }
            }


        }

        private void btn_form_inv_Click(object sender, RoutedEventArgs e)
        {
            form_inv.Visibility = Visibility.Visible;
            form_ing_egre.Visibility = Visibility.Hidden;
        }
        private void btn_cancelar_pro_Click(object sender, RoutedEventArgs e)
        {
            btn_guardar.Visibility = Visibility.Hidden;
            btn_cancelar.Visibility = Visibility.Hidden;
            btn_addCli.Visibility = Visibility.Visible;
            CleanControles();
        }
        private void btn_cancelar_inv_Click(object sender, RoutedEventArgs e)
        {
            form_ing_egre.Visibility = Visibility.Visible;
            form_inv.Visibility = Visibility.Hidden;
            form_stock.Visibility = Visibility.Hidden;
            CleanControlesInv();
        }
        public void CleanControlesInv()
        {
            tb_cod_inv.Clear();
            cbx_categoria_inv.IsEnabled = true;
            cbx_pro_inv.IsEnabled = true;

            cbx_categoria_inv.SelectedIndex = 0;
            cbx_pro_inv.SelectedIndex = 0;

            cbx_ubicacion.SelectedIndex = 0;
            cbx_unimedida_inv.SelectedIndex = 0;

            tb_stock_min.Clear();
            tb_stock_total.Clear();
            tb_stock_total.IsEnabled = true;

            validar_nombre_inv.Content = "";
            validar_categoria_inv.Content = "";
            validar_precio_inv.Content = "";



        }

        private void btn_guardar_inv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InventarioDTO inv = new InventarioDTO();
                inv.cod_inv = int.Parse(tb_cod_inv.Text);
                inv.cod_pro = int.Parse(cbx_pro_inv.SelectedValue.ToString());

                inv.stock_minimo = tb_stock_min.Text;
                inv.stock_total = tb_stock_total.Text;

                inv.idubicacion = int.Parse(cbx_ubicacion.SelectedValue.ToString());
                inv.iduni_medida = int.Parse(cbx_unimedida_inv.SelectedValue.ToString());
                inv.idcategoria = (int)cbx_categoria_inv.SelectedValue;

                if (controller.EditarInventario(inv) > 0)
                {
                    var text = tb_filtro_inv.Text.ToString().ToLower().Split(' ');


                    var list = controller.geInventario();

                    for (int i = 0; i < text.Length; i++)
                    {
                        text[i] = sintilde(text[i]);
                        list = list.Where(x => sintilde(x.nombre.ToString()).ToLower().Replace(" ", "").Contains(text[i])).ToList();

                    }
                    gridinv.ItemsSource = list;
                    btn_guardar_inv.Visibility = Visibility.Hidden;
                    btn_cancelar_inv.Visibility = Visibility.Hidden;
                    btn_save_inv.Visibility = Visibility.Visible;

                    CleanControlesInv();
                    MessageBox.Show("Inventario actualizado");


                }
                else
                {
                    var text = tb_filtro_inv.Text.ToString().ToLower().Split(' ');


                    var list = controller.geInventario();

                    for (int i = 0; i < text.Length; i++)
                    {
                        text[i] = sintilde(text[i]);
                        list = list.Where(x => sintilde(x.nombre.ToString()).ToLower().Replace(" ", "").Contains(text[i])).ToList();

                    }
                    gridinv.ItemsSource = list;
                    btn_guardar_inv.Visibility = Visibility.Hidden;
                    btn_cancelar_inv.Visibility = Visibility.Hidden;
                    btn_save_inv.Visibility = Visibility.Visible;

                    CleanControlesInv();
                    MessageBox.Show("Inventario sin cambios");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void btn_save_inv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InventarioDTO inv = new InventarioDTO();
                inv.cod_pro = int.Parse(cbx_pro_inv.SelectedValue.ToString());

                inv.stock_minimo = tb_stock_min.Text;
                inv.stock_total = tb_stock_total.Text;

                inv.idubicacion = int.Parse(cbx_ubicacion.SelectedValue.ToString());
                inv.iduni_medida = int.Parse(cbx_unimedida_inv.SelectedValue.ToString()) == 0 ? 3 : int.Parse(cbx_unimedida_inv.SelectedValue.ToString());
                inv.idcategoria = (int)cbx_categoria_inv.SelectedValue;

                if (controller.GuardarInventario(inv) > 0)
                {
                    var text = tb_filtro_inv.Text.ToString().ToLower().Split(' ');


                    var list = controller.geInventario();

                    for (int i = 0; i < text.Length; i++)
                    {
                        text[i] = sintilde(text[i]);
                        list = list.Where(x => sintilde(x.nombre.ToString()).ToLower().Replace(" ", "").Contains(text[i])).ToList();

                    }
                    gridinv.ItemsSource = list;
                    form_ing_egre.Visibility = Visibility.Visible;
                    form_inv.Visibility = Visibility.Hidden;
                    CleanControlesInv();
                    MessageBox.Show("Producto Agregado");


                }
                else
                {
                    var text = tb_filtro_inv.Text.ToString().ToLower().Split(' ');


                    var list = controller.geInventario();

                    for (int i = 0; i < text.Length; i++)
                    {
                        text[i] = sintilde(text[i]);
                        list = list.Where(x => sintilde(x.nombre.ToString()).ToLower().Replace(" ", "").Contains(text[i])).ToList();

                    }
                    gridinv.ItemsSource = list;
                    form_ing_egre.Visibility = Visibility.Visible;
                    form_inv.Visibility = Visibility.Hidden;
                    CleanControlesInv();
                    MessageBox.Show("Ocurrio un error");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void btn_menos_Click(object sender, RoutedEventArgs e)
        {
            lab_operacion.Content = "EGRESO DE STOCK";

            form_ing_egre.Visibility = Visibility.Hidden;
            form_inv.Visibility = Visibility.Hidden;
            form_stock.Visibility = Visibility.Visible;

            this.clearControlStock();

            InventarioDTO inv = (InventarioDTO)gridinv.SelectedItem;

            tb_sku.Text = inv.cod_pro.ToString();
            tb_producto.Text = inv.nombre;
            tb_medida.Text = inv.uni_medida;
            tb_ubicacion.Text = inv.ubicacion;
            tb_stock_actual.Text = inv.stock_total;

        }

        private void btn_mas_Click(object sender, RoutedEventArgs e)
        {
            lab_operacion.Content = "INGRESO DE STOCK";

            form_ing_egre.Visibility = Visibility.Hidden;
            form_inv.Visibility = Visibility.Hidden;
            form_stock.Visibility = Visibility.Visible;

            this.clearControlStock();

            InventarioDTO inv = (InventarioDTO)gridinv.SelectedItem;

            tb_sku.Text = inv.cod_pro.ToString();
            tb_producto.Text = inv.nombre;
            tb_medida.Text = inv.uni_medida;
            tb_ubicacion.Text = inv.ubicacion;
            tb_stock_actual.Text = inv.stock_total;
        }

        private void btn_cancelar_stock_Click(object sender, RoutedEventArgs e)
        {
            form_ing_egre.Visibility = Visibility.Visible;
            form_inv.Visibility = Visibility.Hidden;
            form_stock.Visibility = Visibility.Hidden;
            this.clearControlStock();
        }
        private void clearControlStock()
        {
            tb_sku.Clear();
            tb_producto.Clear();
            tb_medida.Clear();
            tb_ubicacion.Clear();
            tb_cant_stock.Clear();
            tb_stock_actual.Clear();
        }
        private void btn_save_stock_Click(object sender, RoutedEventArgs e)
        {
            operacion operacion = new operacion();
            operacion.fecha_hora = DateTime.Now;

            InventarioDTO inv = (InventarioDTO)gridinv.SelectedItem;
            operacion.idinventario = inv.cod_inv;
            if (inv.idcategoria == 6)
            {
                operacion.idingrediente = inv.cod_pro;
            }
            else
            {
                operacion.idproducto = inv.cod_pro;
            }
            string cantidad = tb_cant_stock.Text;
            int stock_ant = 0;
            int stock_actual = 0;
            string mensaje = "";
            if (lab_operacion.Content.ToString() == "INGRESO DE STOCK")
            {
                mensaje = "INGRESO DE STOCK REALIZADO";
                operacion.idtipo_operacion = 2;
                if (inv.iduni_medida == 1)
                {
                    cantidad = cantidad.Replace(".", ",");
                    inv.stock_total = inv.stock_total.Replace(".", ",");

                    operacion.cantidad = (int)(decimal.Parse(cantidad) * 1000);
                    stock_ant = (int)(decimal.Parse(inv.stock_total) * 1000);
                    stock_actual = (int)(decimal.Parse(inv.stock_total) * 1000) + (int)(decimal.Parse(cantidad) * 1000);
                }
                else
                {
                    operacion.cantidad = int.Parse(cantidad);
                    stock_ant = int.Parse(inv.stock_total);
                    stock_actual = int.Parse(inv.stock_total) + int.Parse(cantidad);
                }
            }
            else
            {
                mensaje = "EGRESO DE STOCK REALIZADO";
                operacion.idtipo_operacion = 3;
                if (inv.iduni_medida == 1)
                {
                    cantidad = cantidad.Replace(".", ",");
                    inv.stock_total = inv.stock_total.Replace(".", ",");

                    operacion.cantidad = (int)(decimal.Parse(cantidad) * 1000);

                    stock_ant = (int)(decimal.Parse(inv.stock_total) * 1000);
                    stock_actual = (int)(decimal.Parse(inv.stock_total) * 1000) - (int)(decimal.Parse(cantidad) * 1000);
                }
                else
                {
                    operacion.cantidad = int.Parse(cantidad);
                    stock_ant = int.Parse(inv.stock_total);
                    stock_actual = int.Parse(inv.stock_total) - +int.Parse(cantidad);
                }
            }
            operacion.stock_ant = stock_ant;
            operacion.stock_act = stock_actual;

            if (stock_actual < 0)
            {
                MessageBox.Show("La cantidad ingresada debe ser Mayor o igual al stock actual");
            }
            else
            {
                model.operacion.Add(operacion);

                if (model.SaveChanges() > 0)
                {
                    actualizarStockInv(inv.cod_inv, operacion.stock_act);
                    btn_cancelar_stock_Click(sender, e);
                    //getInv();
                    var text = tb_filtro_inv.Text.ToString().ToLower();


                    var list = controller.geInventario();

                    list = list
                    .Where(x => x.cod_pro.ToString() == text || x.ean.ToLower() == text || sintilde(x.nombre.ToString())
                    .ToLower().Replace(" ", "").Contains(text) == true).ToList();

                    gridinv.ItemsSource = list;
                    MessageBox.Show(mensaje);
                }
                else
                {
                    MessageBox.Show("Hubo problemas");
                }
            }
        }
        private bool actualizarStockInv(int idinventario, int cantidad)
        {
            bool resultado = false;
            inventario inv = model.inventario.FirstOrDefault(x => x.idinventario == idinventario);
            inv.stock_total = cantidad;
            if (model.SaveChanges() > 0)
            {
                resultado = true;
            }
            else
            {
                resultado = false;
            }
            return resultado;
        }

        private void TextBox_TextChanged(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                FiltrarProducto();
            }

        }
        private void FiltrarProducto()
        {
            var text = tb_filtro_producto.Text.ToString().ToLower().Split(' ');


            var list = controller.getGrillaProductos(null);

            for (int i = 0; i < text.Length; i++)
            {
                text[i] = sintilde(text[i]);
                list = list.Where(x => sintilde(x.marca).ToLower().Replace(" ", "").Contains(text[i])
                || sintilde(x.nombre).ToLower().Replace(" ", "").Contains(text[i])
                || sintilde(x.color).ToLower().Replace(" ", "").Contains(text[i])
                || getText(x.ean) == text[i]).ToList();

            }

            dgrid_productos.ItemsSource = list;
        }
        private string getText(string value)
        {
            if (value==null)
            {
                return "";
            }
            else
            {
                return value.ToLower();
            }

        }
        private string getNum(long? text)
        {
            if (text == null)
            {
                return "";
            }
            else
            {
                return text.ToString();
            }
        }

        private string sintilde(string text)
        {
            if (text == null)
            {
                return "";
            }
            else
            {
                return Regex.Replace(text.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "");
            }
        }



        

        private void tb_filtro_inv_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var text = tb_filtro_inv.Text.ToString().ToLower();

                var list = controller.geInventario();
               
                list = list
                    .Where(x => x.cod_pro.ToString() == text || x.ean.ToLower() == text || sintilde(x.nombre.ToString())
                    .ToLower().Replace(" ", "").Contains(text)==true).ToList();

                


                gridinv.ItemsSource = list;
            }
        }
        private void cargarMarca()
        {
            var lis = new List<marca>();
            lis.Add(new marca() { idmarca = 0, nombre = "SELECCIONE" });
            foreach (var item in model.marca.ToList())
            {
                lis.Add(item);
            }
            cbx_marca.ItemsSource = lis;
        }
        private void cargarColor()
        {
            var lis = new List<color>();
            lis.Add(new color() { idcolor = 0, nombre = "SELECCIONE" });
            foreach (var item in model.color.ToList())
            {
                lis.Add(item);
            }
            cbx_color.ItemsSource = lis;
        }

        private void btn_buscar_Click(object sender, RoutedEventArgs e)
        {
            gridmovimiento.ItemsSource = controller.getMovimientoxfecha(fecha_ini.SelectedDate.Value, fecha_fin.SelectedDate.Value,tbx_producto.Text,cbx_tipo_mov.SelectedValue.ToString());

        }

        //private void fecha_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    gridmovimiento.ItemsSource = controller.getMovimientoxfecha(fecha_fin.SelectedDate.Value, fecha_fin.SelectedDate.Value, tbx_producto.Text);
        //}

        private void btn_add_mar_Click(object sender, RoutedEventArgs e)
        {
            ModalItem modalItem = new ModalItem("Ingrese marca");
            var rs = modalItem.ShowDialog();
            if (rs == true)
            {
                cargarMarca();
                cbx_marca.SelectedValue = modalItem.id;
                ModalMensaje modal = new ModalMensaje("Registro realizado", null, true);
                modal.ShowDialog();
            }
            else if (rs == false)
            {
                ModalMensaje modal = new ModalMensaje("Registro no realizado", null, true);
                modal.ShowDialog();
            }
        }

        private void btn_add_color_Click(object sender, RoutedEventArgs e)
        {
            ModalItem modalItem = new ModalItem("Ingrese color");
            var rs = modalItem.ShowDialog();
            if (rs == true)
            {
                cargarColor();
                cbx_color.SelectedValue = modalItem.id;
                ModalMensaje modal = new ModalMensaje("Registro realizado", null, true);
                modal.ShowDialog();
            }
            else if (rs == false)
            {
                ModalMensaje modal = new ModalMensaje("Registro no realizado", null, true);
                modal.ShowDialog();
            }

        }

        private void AgregarProductoTransferencia_Click(object sender, RoutedEventArgs e)
        {
            string msg = validarDesdeHasta();
            if (msg == "")
            {
                centro cen = ((centro)cbx_desde.SelectedItem);
                centro cenDes = ((centro)cbx_hasta.SelectedItem);
                int idcentro = cen.idcentro;
                string centro = cen.nombre;

                List<DetalleDTO> lis = new List<DetalleDTO>();
                foreach (var item in produc_transfer)
                {
                    lis.Add(item);
                }

                ModalProductos modal = new ModalProductos(idcentro, cenDes.idcentro,cenDes.nombre, centro, lis);
                modal.ShowDialog();

                foreach (var item in modal.pro_retorna)
                {
                    produc_transfer.Add(item);
                }
                if (produc_transfer.Count>0)
                {
                    cbx_desde.IsEnabled = false;
                    cbx_hasta.IsEnabled = false;
                }
            }
            else
            {
                ModalMensaje modal = new ModalMensaje(msg);
                modal.ShowDialog();
            }
        }
        public string validarDesdeHasta()
        {
            if (cbx_desde.SelectedIndex == 0)
            {
                return "Debe seleccionar el centro de inicio";
            }
            if (cbx_hasta.SelectedIndex == 0)
            {
                return "Debe seleccionar el centro destino";
            }
            if (cbx_desde.SelectedIndex == cbx_hasta.SelectedIndex)
            {
                return "El centro destino debe ser distinto al de inicio";
            }
            return "";
        }

        private void btn_pro_tran_eliminar_Click(object sender, RoutedEventArgs e)
        {
            ModalMensaje modalMensaje = new ModalMensaje("¿Desea eliminar el producto?", true);
            if (modalMensaje.ShowDialog() == true)
                produc_transfer.Remove((DetalleDTO)tabla_tranfer_pro.SelectedItem);
        }

        private void RealizarTransferencia_Click(object sender, RoutedEventArgs e)
        {
            bool val = false;
            string error = "";
            if (cbx_desde.SelectedIndex == 0)
            {
                error += "Debe seleccionar el centro de inicio \n";
                val = true;
            }
            if (cbx_hasta.SelectedIndex == 0)
            {
                error += "Debe seleccionar el centro destino \n";
                val = true;
            }
            if (produc_transfer.Count == 0)
            {
                error += "Debe seleccionar los productos \n";
                val = true;
            }
            if (string.IsNullOrWhiteSpace(tbx_comentario.Text))
            {
                error += "Debe ingresar un comentario \n";
                val = true;
            }
            if (!val)
            {
                ModalMensaje mens = new ModalMensaje("¿Desea realizar la transferencia?", true);
                var r = mens.ShowDialog();
                if (r==true)
                {
                    int idtransferencia = 0;
                    //crear transferencia
                    idtransferencia = CrearTransferencia();
                    foreach (var item in produc_transfer)
                    {

                        //ingreso stock salida
                        operacion operacionIngre = new operacion();
                        operacionIngre.idproducto = item.codigo;
                        operacionIngre.fecha_hora = DateTime.Now;
                        operacionIngre.idtipo_operacion = 9;
                        operacionIngre.idinventario = item.idinventario.Value;
                        operacionIngre.cantidad = (int)item.total;
                        operacionIngre.stock_act = (int)item.stock - (int)item.total;
                        operacionIngre.stock_ant = (int)item.stock;
                        operacionIngre.idtrans_bodega = idtransferencia;
                        model.operacion.Add(operacionIngre);
                        model.SaveChanges();
                        actualizarStockInv(item.idinventario.Value, operacionIngre.stock_act);
                        val = true;
                    }
                    foreach (var item in produc_transfer)
                    {
                        //ingreso stock entrada
                        operacion operacionSalida = new operacion();
                        var inv = model.inventario.FirstOrDefault(x => x.idproducto == item.codigo &&
                        x.idcentro == ((centro)cbx_hasta.SelectedItem).idcentro);
                        operacionSalida.idproducto = item.codigo;
                        operacionSalida.fecha_hora = DateTime.Now;
                        operacionSalida.idtipo_operacion = 10;
                        operacionSalida.idinventario = inv.idinventario;
                        operacionSalida.cantidad = (int)item.total;
                        operacionSalida.stock_act = (int)inv.stock_total + (int)item.total;
                        operacionSalida.stock_ant = (int)inv.stock_total;
                        operacionSalida.idtrans_bodega = idtransferencia;
                        model.operacion.Add(operacionSalida);
                        model.SaveChanges();
                        actualizarStockInv(inv.idinventario, operacionSalida.stock_act);
                        val = true;
                    }
                    if (val)
                    {
                        string ticket = PrintTicketTransferencia(idtransferencia, tbx_comentario.Text, ((centro)cbx_hasta.SelectedItem).nombre);
                        PrintTransferencia(ticket);
                        ModalMensaje men = new ModalMensaje("Transferencia realizada");
                        men.ShowDialog();
                        limpiarTransferencia();
                    }
                }

            }
            else
            {
                ModalMensaje men = new ModalMensaje(error);
                men.ShowDialog();
            }
        }
        public void limpiarTransferencia()
        {
            produc_transfer.Clear();
            tbx_comentario.Text = string.Empty;
            cbx_desde.IsEnabled = true;
            cbx_hasta.IsEnabled = true;
            cbx_desde.SelectedIndex = 0;
            cbx_hasta.SelectedIndex = 0;

        }
        public void PrintTransferencia(string ticket)
        {
          
            PrintDocument p = new PrintDocument();
            System.Drawing.FontFamily font = new System.Drawing.FontFamily("Courier New");
            
            p.PrintPage += delegate (object sender1, PrintPageEventArgs e1)
            {
                e1.Graphics.DrawString(ticket, new Font(font,12,System.Drawing.FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), new RectangleF(0, 0, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height));

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
        public int CrearTransferencia()
        {
            transferencia_bodega transferencia_ = new transferencia_bodega();
            transferencia_.idcentrodesde = ((centro)cbx_desde.SelectedItem).idcentro;
            transferencia_.idcentrohasta = ((centro)cbx_hasta.SelectedItem).idcentro;
            transferencia_.descripcion = tbx_comentario.Text;
            transferencia_.fecharevision = DateTime.Now;
            transferencia_.login_user_revisa = GlobalClass.idusuario;
            model.transferencia_bodega.Add(transferencia_);
            model.SaveChanges();
            return transferencia_.idtrans_bodega;
        }
        public string PrintTicketTransferencia(int idtransferencia,string comentario, string centroNombre)
        {
            StringBuilder textoComanda = new StringBuilder();

            string tot = new string(' ', 20);
            string str = new string(' ', 10);
            string com = new string(' ', 2);
            string space = new string(' ', 8);
            string spac = new string(' ', 4);
            string linea = new string('_', 25);

            textoComanda.AppendLine(str + "FERRETERIA SAN ALFONSO" + str);
            textoComanda.AppendLine(str);
            textoComanda.AppendLine(str + "TRANSFERENCIA BODEGA" + str);
            textoComanda.AppendLine(str);
            textoComanda.AppendLine("COMENTARIO: " + comentario + str);
            textoComanda.AppendLine(str);
            textoComanda.AppendLine("CENTRO DESTINO: " + centroNombre.ToUpper()+ str);
            textoComanda.AppendLine("N° : " + idtransferencia);
            textoComanda.AppendLine("FECHA Y HORA: " + DateTime.Now);
            textoComanda.AppendLine(linea);
            textoComanda.AppendLine(str);
            textoComanda.AppendLine("CANTIDAD" + space + "PRODUCTO");
            foreach (var item in produc_transfer)
            {
                textoComanda.AppendLine( item.total + space + item.producto );
            }
            textoComanda.AppendLine(linea);
            textoComanda.AppendLine(str + "DEBE FIRMARSE" + str);
            textoComanda.AppendLine(str);

            return textoComanda.ToString();
        }

        private void CancelarTransferencia_Click(object sender, RoutedEventArgs e)
        {

            ModalMensaje m = new ModalMensaje("¿Desea cancelar la transferencia?", true);
            if (m.ShowDialog()==true)
            {
                limpiarTransferencia();
            }

        }

        private async void btn_estado_producto_Click(object sender, RoutedEventArgs e)
        {
            var cod = ((productoDTO)dgrid_productos.SelectedItem).codigo;

            Producto producto = await model.Producto.FirstOrDefaultAsync(x=>x.codigo==cod);
            string msj = "";
            if ((sender as CheckBox).IsChecked == true)
            {
                ((productoDTO)dgrid_productos.SelectedItem).estado = 1;
                producto.estado = 1;
                msj = "Producto Activado";
            }
            else
            {
                ((productoDTO)dgrid_productos.SelectedItem).estado = 0;
                msj = "Producto Desactivado";
                producto.estado = 0;
            }

            int r = await model.SaveChangesAsync();
            ModalMensaje ms = new ModalMensaje(msj, true, true);
            ms.ShowDialog();

        }

        private void cbx_bodega_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var text = tb_filtro_inv.Text.ToString().ToLower();
            var idubi = int.Parse( cbx_bodega.SelectedValue.ToString());

            
            if (!string.IsNullOrWhiteSpace(text))
            {
                var list = controller.geInventario();
                    list = list
                        .Where(x =>
                        x.cod_pro.ToString() == text || x.ean == text || x.idubicacion == idubi ||
                        sintilde(x.nombre.ToString()).ToLower().Replace(" ", "").Contains(text) && (x.idubicacion == idubi || idubi == 0)).ToList();

                
                gridinv.ItemsSource = list;
            }


            
        }
        private void UploadImage_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Title = "Selecciona una imagen",
                Filter = "Imágenes|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Todos los archivos|*.*",
                Multiselect = false
            };

            if (ofd.ShowDialog() == true)
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad; // permite cerrar el archivo después de cargar
                    bitmap.UriSource = new Uri(ofd.FileName);
                    bitmap.EndInit();
                    bitmap.Freeze(); // mejora rendimiento en UI

                    PreviewImage.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"No se pudo cargar la imagen:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void PreviewImage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Crear una nueva ventana para mostrar la imagen grande
            var ventana = new System.Windows.Window
            {
                Title = "Vista previa",
                Width = 600,
                Height = 400,
                Content = new System.Windows.Controls.Image
                {
                    Source = PreviewImage.Source,   // reutiliza la imagen ya cargada
                    Stretch = System.Windows.Media.Stretch.Uniform
                }
            };

            ventana.ShowDialog();
        }
        private byte[] ConvertImageToBytes(System.Windows.Media.ImageSource imageSource)
        {
            var bitmapSource = imageSource as System.Windows.Media.Imaging.BitmapSource;
            if (bitmapSource == null) return null;

            // Usamos un encoder (PNG, JPG, etc.)
            var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
            encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bitmapSource));

            using (var stream = new System.IO.MemoryStream())
            {
                encoder.Save(stream);
                return stream.ToArray(); // 👉 aquí tienes el byte[]
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
    }
    class Item
    {
        public int codigo { get; set; }
        public string nombre { get; set; }
        public Item()
        {
            this.codigo = 0;
            this.nombre = "- SELECCIONE -";
        }

    }
}
