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
    public partial class ModalPc : MetroWindow
    {
        ModelDataBase model = new ModelDataBase();
        public ModalPc()
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowStyle = WindowStyle.None;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.ShowCloseButton = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string id =  GlobalClass.IdComputador;
            if (string.IsNullOrEmpty(tb_nombre.Text))
            {
                validar_nombre.Content = "*";
            }
            else
            {
                var obj = model.Computador.FirstOrDefault(x=>x.nombre==tb_nombre.Text);
                if (obj == null)
                {
                    Computador pc = new Computador();
                    pc.idComputador = id;
                    pc.nombre = tb_nombre.Text;
                    model.Computador.Add(pc);
                    int n = model.SaveChanges();
                    if (n > 0)
                    {
                        GlobalClass.NameComputador = pc.nombre;
                        GlobalClass.IdComputador = pc.idComputador;
                        DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        DialogResult = false;
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("El nombre existe");
                }
                
            }
        }

        private void btn_cerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
