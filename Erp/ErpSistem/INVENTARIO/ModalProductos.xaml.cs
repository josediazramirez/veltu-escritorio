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
using System.Collections.ObjectModel;

namespace ErpClass
{
    /// <summary>
    /// Lógica de interacción para MyModal.xaml
    /// </summary>
    public partial class ModalProductos : MetroWindow
    {
        ModelDataBase db = new ModelDataBase();
        public ObservableCollection<DetalleDTO> productos = new ObservableCollection<DetalleDTO>();
        private List<DetalleDTO> pro_add = new List<DetalleDTO>();
        public List<DetalleDTO> pro_retorna = new List<DetalleDTO>();
        public int idcentro { get; set; }
        public int idcentroDes { get; set; }
        public string _centroNomDes { get; set; }
        public ModalProductos(int _idcentro,int _idcentroDes,string _centroNom,string centro,List<DetalleDTO> prod)
        {

            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowStyle = WindowStyle.None;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.ShowCloseButton = false;
            tabla_pro.ItemsSource = productos;
            idcentro = _idcentro;
            pro_add = prod;
            txt_centro.Content = centro.ToUpper();
            idcentroDes = _idcentroDes;
            _centroNomDes = _centroNom;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void txt_filtro_pro_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var text =((TextBox) sender).Text.ToString().ToLower();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    productos.Clear();
                    ProductoController fn_pro = new ProductoController();
                    var list = fn_pro.getFiltroProductoFull(text,idcentro).Select(x => new DetalleDTO
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
                        idmedida = x.idmedida
                    }).ToList();

                    if (list.Count > 0)
                    {
                        //vienden de al lista
                        foreach (var item in pro_add)
                        {
                            var obj = list.FirstOrDefault(x => x.codigo == item.codigo);
                            if(obj!=null)obj.Visible = Visibility.Collapsed;
                        }
                        //vienden del modal
                        foreach (var item in pro_retorna)
                        {
                            var obj = list.FirstOrDefault(x => x.codigo == item.codigo);
                            if (obj != null) obj.Visible = Visibility.Collapsed;
                        }
                        foreach (var item in list)
                        {
                            productos.Add(item);
                        } 
                    }
                    else
                    {
                        msj("Producto no encontrado", null, true);
                    }
                }
                else
                {
                    tabla_pro.ItemsSource = null;
                }
            }
        }
        private bool? msj(string msj, bool? canc = null, bool? time = null)
        {
            ModalMensaje m = new ModalMensaje(msj, canc, time);
            return m.ShowDialog();
        }

        private void btn_agregarPro_Click(object sender, RoutedEventArgs e)
        {
            DetalleDTO det = (DetalleDTO)tabla_pro.SelectedItem;
            ModalCantidadPro mod = new ModalCantidadPro();
            var bol = mod.ShowDialog();
            if (bol==true)
            {
                if (mod.Cantidad<= det.stock)
                {
                    var pro = db.inventario.FirstOrDefault(x => x.idproducto == det.codigo && x.idcentro == idcentroDes);
                    if (pro==null)
                    {
                        ModalMensaje moda = new ModalMensaje("El producto ["+det.codigo+"] "+det.producto+" no existe en el centro destino "+_centroNomDes);
                        moda.ShowDialog();
                    }
                    else
                    {
                        var obj = productos.FirstOrDefault(x => x.codigo == det.codigo);
                        det.Visible = Visibility.Collapsed;
                        det.total = mod.Cantidad;
                        pro_retorna.Add(det);
                    }

                }
                else
                {
                    ModalMensaje moda = new ModalMensaje("La cantidad debe ser menor al stock");
                    moda.ShowDialog();
                }

            }

        }
    }
}
