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
    public partial class ModalItem : MetroWindow
    { 
        public int id = 0;
        readonly ModelDataBase db = new ModelDataBase();
        public ModalItem(string titulo)
        {
            InitializeComponent();
            if (App.Current.MainWindow.IsActive)
            {
                this.Owner = App.Current.MainWindow;
            } 
            lb_texto.Text = titulo;
            
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            object obj = null;
            string val = "";
            int tipo = 0;
            if (lb_texto.Text.Contains("color"))
            {
                 obj = db.color.FirstOrDefault(x => x.nombre == tb_texto.Text);
                val = "El color ya existe";
                tipo = 1;
            }
            else
            {
                 obj = db.marca.FirstOrDefault(x => x.nombre==tb_texto.Text);
                val = "La marca ya existe";
                tipo = 2;
            }
           
            if (obj != null)
            {
                ModalMensaje modal = new ModalMensaje(val);
                modal.ShowDialog();
            }
            else
            {
                int rs = 0;
                if (tipo==1)
                {
                    var co = new color() { nombre = tb_texto.Text };
                    db.color.Add(co);
                     rs = db.SaveChanges();
                    id = co.idcolor;
                }
                else
                {
                    var ma = new marca() { nombre = tb_texto.Text };
                    db.marca.Add(ma);
                     rs = db.SaveChanges();
                    id = ma.idmarca;
                }
                
                
                if (rs>0)
                {
                    DialogResult = true;
                    
                    this.Close();
                }
                else
                {
                    DialogResult = false;
                    this.Close();
                }
                
            }

        }

        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = null;
            this.Close();
        }
    }
}
