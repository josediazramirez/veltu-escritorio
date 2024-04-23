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
    public partial class ModalCantidad : MetroWindow
    { 
        public string mensaje ="";
        public int total { get; set; }
        public ModalCantidad()
        {
            InitializeComponent();
            if (App.Current.MainWindow.IsActive)
            {
                this.Owner = App.Current.MainWindow;
            } 

            this.ShowCloseButton = false;
            
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int num = 0;
            if (int.TryParse(txt_total.Text,out num))
            {
                total = num;
                DialogResult = true;
                this.Close();
            }
            else
            {
                ModalMensaje modal = new ModalMensaje("Debe ingresar la cantidad", null, true);
                modal.ShowDialog();
            }
        }

        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
