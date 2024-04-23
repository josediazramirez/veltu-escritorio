using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace ErpClass
{
    /// <summary>
    /// Lógica de interacción para MyModal.xaml
    /// </summary>
    public partial class ModalAperturaCierre : MetroWindow
    {
        public int total = 0;
        public ModalAperturaCierre()
        {
            
            InitializeComponent();
            this.Owner = App.Current.MainWindow;
            this.ShowCloseButton = false;

        }

        private int getNumber(TextBox text)
        {
            string valor = text.Text.Replace(".", "").ToString();
            int num = 0;
            if (int.TryParse(valor, out num))
            {
                return num;
            }
            return num;
        }

        private void formatoInputNumber(TextBox textBox)
        {
            if (getNumber(textBox)==0)
            {
                textBox.Text = "";
            }
            else
            {
                textBox.Text = String.Format("{0:N0}",getNumber(textBox));
            }
        }
        private void calcularTotal()
        {
            int bi20mil = getNumber(txt_20000);

            int total = bi20mil;

            lb_total.Content = String.Format("{0:N0}", total);

           // formatoInputNumber(txt_20000);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }


        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_aceptar_Click(object sender, RoutedEventArgs e)
        {
            string valor = lb_total.Content.ToString().Replace(".", "").ToString();
            int num = 0;
            if (int.TryParse(valor, out num))
            {
                total = num;
            }
            else
            {
                total = num;
            }
            DialogResult = true;
            this.Close();
            
        }

        private void txt_20000_KeyUp(object sender, KeyEventArgs e)
        {
            calcularTotal();
        }
    }
}
