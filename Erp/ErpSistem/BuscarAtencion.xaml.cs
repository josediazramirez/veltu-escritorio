using MahApps.Metro.Controls;
using Model;
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

namespace ErpClass
{
    /// <summary>
    /// Lógica de interacción para ModalPc.xaml
    /// </summary>
    public partial class BuscarAtencion : Window
    { 
        readonly ModelDataBase db = new ModelDataBase();
        public int _idatencion = 0;
        public BuscarAtencion()
        {
            InitializeComponent();
            if (App.Current.MainWindow.IsActive)
            {
                this.Owner = App.Current.MainWindow;
            }
            this.txt_clave.Focus();

        }
        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txt_clave_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string key = (sender as TextBox).Text;
                int n = 0;
                if (int.TryParse(key,out n))
                {
                    _idatencion = n;
                    DialogResult = true;
                }
                else
                {
                    DialogResult = false;
                }
            }
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txt_clave.Focus();
        }

        private void MetroWindow_KeyUp(object sender, KeyEventArgs e)
        {
            txt_clave.Focus();
        }
    }
}
