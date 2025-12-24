using Controller;
using MahApps.Metro.Controls;
using Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ErpClass
{
    /// <summary>
    /// Lógica de interacción para Home.xaml
    /// </summary>
    public partial class Home : Window
    {

        FuncionesUtiles funciones = new FuncionesUtiles();
        ModelDataBase db = new ModelDataBase();
        public Home()
        {
            InitializeComponent();
           StatusDB();
            
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Configura el tamaño de la ventana para no cubrir la barra de tareas
            var workingArea = System.Windows.SystemParameters.WorkArea;

            this.Top = workingArea.Top;
            this.Left = workingArea.Left;
            this.Width = workingArea.Width;
            this.Height = workingArea.Height;
        }


        public void StatusDB()
        {
            if (!funciones.CheckConnection())
            {
                mensaje("Base de datos no iniciada");
                mensaje("Inicie base de datos");
                this.Close();
            }
            else
            {

                Login l = new Login();
                base_home.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;
                base_home.Navigate(l);
            }
        }
        private bool mensaje(string msj,bool? confirm=null)
        {
            ModalMensaje mensaje = new ModalMensaje(msj,confirm);
            if (mensaje.ShowDialog()==true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var content = (Page)base_home.Content;

            if (content!=null)
            {
                if (content.Name == "form_login")
                {
                    bool confi = mensaje("¿Desea cerrar el sistema?", true);
                    if (confi)
                    {
                        e.Cancel = false;
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
                else
                {
                    bool confi = mensaje("¿Desea cerrar la sesión?", true);
                    if (confi)
                    {
                        e.Cancel = true;
                        Login l = new Login();
                        base_home.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;
                        base_home.Navigate(l);
                    }
                    else
                    {
                        e.Cancel = true;
                    }

                }
            }
            else
            {
                e.Cancel = false;
            }
        }


        private void MetroWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
