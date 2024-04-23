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
    public partial class HabilitarProceso : Window
    { 
        public string mensaje ="";
        readonly ModelDataBase db = new ModelDataBase();
        private int _idproceso = 0;
        public HabilitarProceso(string mensaje,int idproceso)
        {
            InitializeComponent();
            if (App.Current.MainWindow.IsActive)
            {
                this.Owner = App.Current.MainWindow;
            }
            this.txt_clave.Focus();
            lb_mensaje.Text = mensaje;
            _idproceso = idproceso; 

        }
        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txt_clave_KeyUp(object sender, KeyEventArgs e)
        {
            /*
                '1','descuento','aplicar descuento','1'
                '2','cancelar venta','cancelar venta','2'
                '3','autoriza cambio','autorizar cambio','3'
                */

            if (e.Key == Key.Enter)
            {
                string key = (sender as TextBox).Text;
                usuario user = db.usuario.FirstOrDefault(x => x.key == key);
                if (user!=null)
                {
                    var acceso = db.usuario_proceso.FirstOrDefault(x => x.proce_id == _idproceso && x.idusuario == user.idusuario);
                    if (acceso != null)
                    {
                        
                        LogAutorizacion("AUTORIZADO", "USUARIO AUTORIZADO", _idproceso, user.idusuario);
                        DialogResult = true;
                    }
                    else
                    {
                        LogAutorizacion("NO AUTORIZADO", "USUARIO NO AUTORIZADO", _idproceso, user.idusuario);
                        DialogResult = false;
                    }
                }else
                {
                    LogAutorizacion("NO AUTORIZADO", "USUARIO NO EXISTE EN EL SISTEMA KEY "+key, _idproceso,null);
                    DialogResult = false;
                }
            }
        }
        public void LogAutorizacion(string _auto_estado,string _msg,int _proceid,int? _idusuario)
        {
            autorizacion_proceso autorizacion_ = new autorizacion_proceso();
            autorizacion_.auto_fecha = DateTime.Now;
            autorizacion_.auto_mensaje = _msg;
            autorizacion_.auto_estado = _auto_estado;
            autorizacion_.proce_id = _proceid;
            autorizacion_.idusuario = _idusuario;

            db.autorizacion_proceso.Add(autorizacion_);

            int resp = db.SaveChanges();



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
