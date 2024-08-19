using Controller;
using DTO;
using MahApps.Metro.Controls;
using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Deployment.Application;
using System.Linq;
using System.Reflection;
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
    public partial class Login : Page
    {
        ModelDataBase db = new ModelDataBase();
        FuncionesUtiles funciones = new FuncionesUtiles();
        public Login()
        {
            InitializeComponent();

            string versionString = "V1.0"; // Valor por defecto

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                var version = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                versionString = $"V{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            }
            MyVersionLabel.Content = $"{versionString}";
        }
        private void trueLoading()
        {
            load_true.Visibility = Visibility.Visible;
        }
        private void falseLoading()
        {

            load_true.Visibility = Visibility.Collapsed;
        }

        private void btn_entrar_Click(object sender, RoutedEventArgs e)
        {
            btn_entrar.IsHitTestVisible = false;
            entrar();
            falseLoading();
            btn_entrar.IsHitTestVisible = true;
        }
        private async void entrar()
        {

            string user = txt_usuario.Text;
            string password = txt_password.Password;
            if (string.IsNullOrWhiteSpace(user))
            {
                mensaje("Debe ingresar el usuario");
            }
            else if (string.IsNullOrWhiteSpace(password))
            {
                mensaje("Debe ingresar la contraseña");
            }
            else
            {
                trueLoading();
                var obj = await GetUsuario(user, password);

                if (obj != null)
                {
                    GlobalClass.user_rol = obj.user_rol_id;
                    GlobalClass.idusuario = obj.idusuario;
                    GlobalClass.user_nombre = obj.user_nombre + " " + obj.ape_paterno;
                    GlobalClass.user_rol_nombre = obj.user_rol_nombre;

                    Menu page = new Menu();
                    NavigationService.Navigate(page);

                }
                else
                {
                    mensaje("Credenciales incorrectas o usuario sin acceso");
                }
            }

        }
        private async Task<userDTO> GetUsuario(string user, string clave)
        {
            Task<userDTO> task = (from us in db.usuario
                                  join ro in db.rol
                                  on us.idrol equals ro.idrol
                                  where us.login_usuario == user &&
                                 us.password == clave
                                 && us.habilitado==1
                                  select new userDTO
                                  {
                                      idusuario = us.idusuario,
                                      user_nombre = us.nombre,
                                      ape_paterno = us.ape_paterno,
                                      user_rol_id = ro.idrol,
                                      user_rol_nombre = ro.nombre
                                  }).FirstOrDefaultAsync();
            
            return await task;
        }
        private void mensaje(string msj)
        {
            ModalMensaje mensaje = new ModalMensaje(msj,null,true);
            mensaje.ShowDialog();
        }


        private void btn_salir_Click(object sender, RoutedEventArgs e)
        {
            var parentWindow = Window.GetWindow(this);

            if (parentWindow != null)
            {
                parentWindow.Close();
            }
        }
    }
}
