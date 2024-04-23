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
    public partial class ModalMensaje : MetroWindow
    { 
        public string mensaje ="";
        public ModalMensaje(string mensaje, bool? confirm=null,bool? time=null)
        {
            InitializeComponent();
            if (App.Current.MainWindow.IsActive)
            {
                this.Owner = App.Current.MainWindow;
            } 

            this.ShowCloseButton = false;
            lb_mensaje.Text = mensaje;
            if (confirm==true)
            {
                btn_cancelar.Visibility = Visibility.Visible;
            }
            if (time==true)
            {
                btn_aceptar.Visibility = Visibility.Collapsed;
                close();
            }
            
        }
        private async void close()
        {
            await Task.Delay(2000);
            this.Close();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
