using MahApps.Metro.Controls;
using Model;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ErpClass
{
    /// <summary>
    /// Lógica de interacción para ModalPc.xaml
    /// </summary>
    public partial class modal_descuento : MetroWindow
    {
        ModelDataBase model = new ModelDataBase();
        public int porcetaje = 0;
        public modal_descuento()
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowStyle = WindowStyle.None;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.ShowCloseButton = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string numero = tb_porcetaje.Text;
            int num = 0;
            if (int.TryParse(numero,out num))
            {
                int cant_ent = num;
                if (cant_ent <= 100)
                {
                    this.porcetaje = cant_ent;
                    DialogResult = true;
                    this.Close();
                }
                else
                {
                    ModalMensaje modal = new ModalMensaje("Debe ingresar una cantidad menor o igual a 100");
                    modal.ShowDialog();
                }
            }
            else
            {
                ModalMensaje modal = new ModalMensaje("Debe ingresar una cantidad");
                modal.ShowDialog();
            }

          
        }

        private void btn_cerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void tb_cant_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }
    }
}
