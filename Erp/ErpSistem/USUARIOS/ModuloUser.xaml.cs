using DTO;
using ErpClass;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ErpAdmin.USUARIOS
{
    /// <summary>
    /// Lógica de interacción para ModuloUser.xaml
    /// </summary>
    public partial class ModuloUser : Page
    {
        ModelDataBase db = new ModelDataBase();
        public ModuloUser()
        {
            InitializeComponent();
            cargarUsuarios();
            cargarRoles();
        }
        public void cargarUsuarios()
        {
            var list = (from u in db.usuario
                        join r in db.rol
                        on u.idrol equals r.idrol
                        select new UsuarioDTO
                        {
                            idusuario = u.idusuario,
                            nombre = u.nombre,
                            ape_ma = u.ape_materno,
                            ape_pa = u.ape_paterno,
                            correo = u.correo,
                            habilitado = u.habilitado,
                            login = u.login_usuario,
                            roles = r.nombre,
                            idrol = r.idrol
                            
                        }).ToList();

            tabla_user.ItemsSource = list;
        }
        private void btn_editar_Click(object sender, RoutedEventArgs e)
        {
            UsuarioDTO user = ((UsuarioDTO)tabla_user.SelectedItem);

            tbx_idusuario.Text = user.idusuario.ToString();
            tbx_login.Text = user.login;
            tbx_nombre.Text = user.nombre;
            tbx_ape_ma.Text = user.ape_ma;
            tbx_ape_pa.Text = user.ape_pa;
            tbx_correo.Text = user.correo;
            cbx_rol.SelectedValue = user.idrol;
            cbx_estado.IsChecked = user.habilitado == 1 ? true : false;

            tbx_clave.Visibility = Visibility.Hidden;
            tbx_confir_clave.Visibility = Visibility.Hidden;

            grilla_usuario.Visibility = Visibility.Visible;
            nuevo_usuario.Visibility = Visibility.Collapsed;

            grilla_clave.Visibility = Visibility.Collapsed;
            tbx_clave_edit.Password = string.Empty;
            tbx_confir_clave_edit.Password = string.Empty;

            label_accion.Text = "Editar Usuario";
        }

        private async void btn_registrar_Click(object sender, RoutedEventArgs e)
        {
            string msj = "";
            if (label_accion.Text == "Editar Usuario") 
            {
                msj= Validar2();

                if (msj == string.Empty)
                {

                    ModalMensaje ms = new ModalMensaje("¿Desea editar el usuario?", true);
                    ms.ShowDialog();
                    if (ms.DialogResult == true)
                    {
                        int idusuario = int.Parse(tbx_idusuario.Text);
                        usuario user = db.usuario.FirstOrDefault(x=>x.idusuario== idusuario);
                        user.login_usuario = tbx_login.Text;
                        user.nombre = tbx_nombre.Text;
                        user.ape_paterno = tbx_ape_pa.Text;
                        user.ape_materno = tbx_ape_ma.Text;
                        user.correo = tbx_correo.Text;
                        user.idrol = (int)cbx_rol.SelectedValue;
                        user.habilitado = cbx_estado.IsChecked==true?1:0;


                       int r = await db.SaveChangesAsync();
                        cargarUsuarios();

                        tbx_clave.Visibility = Visibility.Visible;
                        tbx_confir_clave.Visibility = Visibility.Visible;

                        grilla_usuario.Visibility = Visibility.Collapsed;
                        nuevo_usuario.Visibility = Visibility.Visible;
                        mensaje("Usuario actualizado");
                        limpiar();
                    }

                }
                else
                {
                    mensaje(msj);
                }
            }
            else
            {
                msj = Validar();

                if (msj == string.Empty)
                {

                    ModalMensaje ms = new ModalMensaje("¿Desea registrar el usuario?", true);
                    ms.ShowDialog();
                    if (ms.DialogResult == true)
                    {
                        usuario usuario = new usuario();
                        usuario.login_usuario = tbx_login.Text;
                        usuario.nombre = tbx_nombre.Text;
                        usuario.ape_paterno = tbx_ape_pa.Text;
                        usuario.ape_materno = tbx_ape_ma.Text;
                        usuario.correo = tbx_correo.Text;
                        usuario.password = tbx_clave.Password;

                        usuario.idrol = (int)cbx_rol.SelectedValue;
                        usuario.habilitado = cbx_estado.IsChecked.Value == true ? 1 : 0;

                        db.usuario.Add(usuario);

                        await db.SaveChangesAsync();
                        cargarUsuarios();
                        mensaje("Usuario registrado");
                        limpiar();
                    }


                }
                else
                {
                    mensaje(msj);
                }
            }

        }
        private void mensaje(string msj)
        {
            ModalMensaje modal = new ModalMensaje(msj);
            modal.ShowDialog();
        }
        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            ModalMensaje ms = new ModalMensaje("¿Desea cancelar el registro?", true);
            ms.ShowDialog();
            if (ms.DialogResult==true)
            {
                grilla_usuario.Visibility = Visibility.Collapsed;
                nuevo_usuario.Visibility = Visibility.Visible;
                limpiar();
            }
        }
        private void cargarRoles()
        {
            List<ComboItemDTO> comboItems = new List<ComboItemDTO>();
            comboItems.Add(new ComboItemDTO { Valor = 0, Name = "Seleccione" });
            comboItems.AddRange(db.rol.Select(x => new ComboItemDTO
            {
                Valor = x.idrol,
                Name = x.nombre
            }).ToList());

            cbx_rol.ItemsSource = comboItems;
        }

        private async void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            int idusuario = ((UsuarioDTO)tabla_user.SelectedItem).idusuario;
            var us = db.usuario.FirstOrDefault(x => x.idusuario == idusuario);
            string msj = "";
            if ((sender as CheckBox).IsChecked==true)
            {
                ((UsuarioDTO)tabla_user.SelectedItem).habilitado = 1;
                us.habilitado = 1;
                msj = "Usuario Activado";
            }
            else
            {
                ((UsuarioDTO)tabla_user.SelectedItem).habilitado = 0;
                msj = "Usuario Desactivado";
                us.habilitado = 0;
            }

           int r = await db.SaveChangesAsync();
            ModalMensaje ms = new ModalMensaje(msj, true,true);
            ms.ShowDialog();
        }
        private string Validar()
        {
            if (string.IsNullOrWhiteSpace(tbx_login.Text))
            {
                return "Debe ingresar el login";
            }
            if (string.IsNullOrWhiteSpace(tbx_nombre.Text))
            {
                return "Debe ingresar el nombre";
            }
            if (string.IsNullOrWhiteSpace(tbx_ape_pa.Text))
            {
                return "Debe ingresar el apellido paterno";
            }
            if (string.IsNullOrWhiteSpace(tbx_clave.Password))
            {
                return "Debe ingresar la clave";
            }
            if (!tbx_confir_clave.Password.Equals(tbx_confir_clave.Password))
            {
                return "Debe confirmar la clave";
            }
            if (cbx_rol.SelectedIndex==0)
            {
                return "Debe seleccionar un rol";
            }
            return "";
        }
        public void limpiar()
        {
            tbx_login.Text = string.Empty;
            tbx_nombre.Text = string.Empty;
            tbx_ape_ma.Text = string.Empty;
            tbx_ape_pa.Text = string.Empty;
            tbx_clave.Password = string.Empty;
            tbx_confir_clave.Password = string.Empty;
            cbx_rol.SelectedIndex = 0;
            cbx_estado.IsChecked = true;
        }

        private void nuevo_usuario_Click(object sender, RoutedEventArgs e)
        {
            grilla_usuario.Visibility = Visibility.Visible;
            nuevo_usuario.Visibility = Visibility.Collapsed;
            label_accion.Text = "Registrar Usuario";
            tbx_confir_clave.Visibility = Visibility.Visible;
            tbx_clave.Visibility = Visibility.Visible;
        }

        private void btn_limpiar_Click(object sender, RoutedEventArgs e)
        {

            ModalMensaje ms = new ModalMensaje("¿Desea limpiar los campos?", true);
            ms.ShowDialog();
            if (ms.DialogResult == true)
            {
                limpiar();
            }
        }
        private string Validar2()
        {
            if (string.IsNullOrWhiteSpace(tbx_login.Text))
            {
                return "Debe ingresar el login";
            }
            if (string.IsNullOrWhiteSpace(tbx_nombre.Text))
            {
                return "Debe ingresar el nombre";
            }
            if (string.IsNullOrWhiteSpace(tbx_ape_pa.Text))
            {
                return "Debe ingresar el apellido paterno";
            }
            if (cbx_rol.SelectedIndex == 0)
            {
                return "Debe seleccionar un rol";
            }
            return "";
        }

        private void btn_cancelar_clave_Click(object sender, RoutedEventArgs e)
        {
            grilla_clave.Visibility = Visibility.Collapsed;
            tbx_clave_edit.Password = string.Empty;
            tbx_confir_clave_edit.Password = string.Empty;

            nuevo_usuario.Visibility = Visibility.Visible;
        }

        private async void btn_registrar_clave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbx_clave_edit.Password)==true)
            {
                mensaje("Debe ingresar la clave");
            }
            else
            {
                if (!tbx_clave_edit.Password.Equals(tbx_confir_clave_edit.Password))
                {
                    mensaje("Debe confirmar la clave");
                }
                else
                {
                    ModalMensaje ms = new ModalMensaje("¿Desea cambiar la clave?", true);
                    ms.ShowDialog();
                    if (ms.DialogResult == true)
                    {
                        int idusuario = int.Parse(tbx_idusuario.Text);
                        usuario user = db.usuario.FirstOrDefault(x => x.idusuario == idusuario);
                        user.password = tbx_clave_edit.Password;



                        int r = await db.SaveChangesAsync();
                        cargarUsuarios();

                        grilla_clave.Visibility = Visibility.Collapsed;
                        nuevo_usuario.Visibility = Visibility.Visible;
                        mensaje("Clave de usuario actualizada");
                    }
                }

            }
        }

        private void btn_cambiar_clave_Click(object sender, RoutedEventArgs e)
        {
            tbx_idusuario.Text = ((UsuarioDTO)tabla_user.SelectedItem).idusuario.ToString();
            grilla_clave.Visibility = Visibility.Visible;
            grilla_usuario.Visibility = Visibility.Collapsed;
            tbx_clave_edit.Password = string.Empty;
            tbx_confir_clave_edit.Password = string.Empty;

            nuevo_usuario.Visibility = Visibility.Collapsed;
        }
    }
}
