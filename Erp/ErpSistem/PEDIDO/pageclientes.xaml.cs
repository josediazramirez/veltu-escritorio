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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Controller;
using DTO;
using System.Data;
using System.Data.Entity.Core;
using System.Windows.Threading;

namespace ErpClass
{
    /// <summary>
    /// Lógica de interacción para pageclientes.xaml
    /// </summary>
    public partial class pageclientes : Page
    {
        ClienteController controller = new ClienteController();
        public pageclientes()
        {
            InitializeComponent();
            GridCliente();
        }
        public void CleanControles()
        {
            tb_nombre.Clear();
            tb_telefono.Clear();
            tb_dirrecion.Clear();
            tb_numdire.Clear();
            tb_telefono_opc.Clear();
            validar_direccion.Content = "";
            validar_nombre.Content = "";
            validar_telefono.Content = "";
            validar_dire_num.Content = "";
        }
        public void limpiarMensajes()
        {
            validar_direccion.Content = "";
            validar_nombre.Content = "";
            validar_telefono.Content = "";
            validar_dire_num.Content = "";
        }
        public void GridCliente()
        {
            try
            {
                dgrid_clientes.ItemsSource = controller.GetClientes();
            }
            catch (EntityException ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        private void button_editar(object sender, RoutedEventArgs e)
        {
            CleanControles();
            clienteDTO cliente = (clienteDTO)dgrid_clientes.SelectedItem;
            idcliente.Text = cliente.idcliente.ToString();
            tb_nombre.Text = cliente.nombre;
            tb_telefono.Text = cliente.id_telefono.ToString();
            tb_dirrecion.Text = cliente.direccion;
            tb_numdire.Text = cliente.num_direccion.ToString();
            tb_telefono_opc.Text = cliente.telefono_opc.ToString();

            btn_guardar.Visibility = Visibility.Visible;
            btn_cancelar.Visibility = Visibility.Visible;
            btn_addCli.Visibility = Visibility.Hidden;
            tb_telefono.IsEnabled = false;
        }
        private void button_guardar(object sender, RoutedEventArgs e)
        {
            limpiarMensajes();
            try
            {
                int num = 0;
                if (int.TryParse(tb_telefono.Text, out num) == false || tb_telefono.Text.Length!=9)
                {
                    validar_telefono.Content = "*";
                }
                else if (string.IsNullOrEmpty(tb_nombre.Text) == true)
                {
                    validar_nombre.Content = "*";
                }
                else if (string.IsNullOrEmpty(tb_dirrecion.Text) == true)
                {
                    validar_direccion.Content = "*";
                }
                else if (int.TryParse(tb_numdire.Text, out num) == false)
                {
                    validar_dire_num.Content = "*";
                }
                else
                {
                    clienteDTO cliente = new clienteDTO();
                    cliente.nombre = tb_nombre.Text.ToLower();
                    cliente.id_telefono = int.Parse(tb_telefono.Text);
                    cliente.direccion = tb_dirrecion.Text.ToLower();
                    cliente.num_direccion = int.Parse(tb_numdire.Text);
                    cliente.idcliente = int.Parse(idcliente.Text);
                     if (int.TryParse(tb_telefono_opc.Text, out num) == true && tb_telefono.Text.Length == 9)
                    {
                        cliente.telefono_opc = int.Parse(tb_telefono_opc.Text);
                    }
                    if (controller.EditCliente(cliente) > 0)
                    {
                        GridCliente();
                        btn_guardar.Visibility = Visibility.Hidden;
                        btn_cancelar.Visibility = Visibility.Hidden;
                        btn_addCli.Visibility = Visibility.Visible;

                        CleanControles();
                        MessageBox.Show("Cliente actualizado");
                    }
                    else
                    {
                        btn_guardar.Visibility = Visibility.Hidden;
                        btn_cancelar.Visibility = Visibility.Hidden;
                        btn_addCli.Visibility = Visibility.Visible;
                        CleanControles();
                        MessageBox.Show("Cliente sin cambios");
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
        private void btn_addCli_Click(object sender, RoutedEventArgs e)
        {
            limpiarMensajes();
            try
            {
                int num = 0;
                if (int.TryParse(tb_telefono.Text, out num) == false || tb_telefono.Text.Length != 9)
                {
                    validar_telefono.Content = "*";
                }
                else if (string.IsNullOrEmpty(tb_nombre.Text) == true)
                {
                    validar_nombre.Content = "*";
                }
                else if (string.IsNullOrEmpty(tb_dirrecion.Text) == true)
                {
                    validar_direccion.Content = "*";
                }
                else if (int.TryParse(tb_numdire.Text, out num) == false)
                {
                    validar_dire_num.Content = "*";
                }
                else
                {
                    clienteDTO cli = new clienteDTO();
                    cli.nombre = tb_nombre.Text.ToLower();
                    cli.direccion = tb_dirrecion.Text.ToLower();
                    cli.id_telefono = int.Parse(tb_telefono.Text);
                    cli.num_direccion = int.Parse(tb_numdire.Text);
                    if (int.TryParse(tb_telefono_opc.Text, out num) == true && tb_telefono.Text.Length == 9)
                    {
                        cli.telefono_opc = int.Parse(tb_telefono_opc.Text);
                    }
                    if (controller.guardarCliente(cli) !=null)
                    {
                        CleanControles();
                        GridCliente();
                        MessageBox.Show("Cliente agregado");
                    }
                    else
                    {
                        CleanControles();
                        MessageBox.Show("Cliente existente");
                    }
                }    
            }
            catch (Exception ex)
            {

                MessageBox.Show( ex.Message);
            }
           
          
           

        }
        public void bt_cancelar(object sender, RoutedEventArgs e)
        {
            btn_guardar.Visibility = Visibility.Hidden;
            btn_cancelar.Visibility = Visibility.Hidden;
            btn_addCli.Visibility = Visibility.Visible;
            CleanControles();
            
        }
    }
}
