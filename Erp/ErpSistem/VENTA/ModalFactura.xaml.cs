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
    public partial class ModalFactura : MetroWindow
    {
        readonly ModelDataBase bd = new ModelDataBase();
        public ModalFactura()
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
            if (GlobalClass.factura!=null)
            {
                txt_correo.Text = GlobalClass.factura.correo;
                txt_numero.Text = GlobalClass.factura.numero.ToString();
                txt_rut.Text = GlobalClass.factura.rut;
            }
            
        }

        private void bt_realizarventa_Click(object sender, RoutedEventArgs e)
        {
            int s = 0;
            
            if (string.IsNullOrEmpty(txt_rut.Text))
            {
                ModalMensaje m = new ModalMensaje("Debe ingresar el rut",null,true);
                m.Show();
            }else if (string.IsNullOrEmpty(txt_correo.Text))
            {
                ModalMensaje m = new ModalMensaje("Debe ingresar el correo", null, true);
                m.Show();
            }
            else if (string.IsNullOrEmpty(txt_numero.Text))
            {
                ModalMensaje m = new ModalMensaje("Debe ingresar el numero", null, true);
                m.Show();
            }
            else if (!int.TryParse(txt_numero.Text,out s))
            {
                ModalMensaje m = new ModalMensaje("Debe ingresar el numero correctamente", null, true);
                m.Show();
            }
            else
            {
               FacturaDTO factura = new FacturaDTO();
                factura.correo = txt_correo.Text;   
                factura.rut = txt_rut.Text;
                factura.numero = int.Parse(txt_numero.Text);
                GlobalClass.factura = factura;
                DialogResult = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
