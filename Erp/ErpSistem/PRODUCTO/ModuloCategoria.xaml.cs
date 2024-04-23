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

namespace ErpAdmin.PRODUCTO
{
    /// <summary>
    /// Lógica de interacción para ModuloUser.xaml
    /// </summary>
    public partial class ModuloCategoria : Page
    {
        ModelDataBase db = new ModelDataBase();
        public ModuloCategoria()
        {
            InitializeComponent();
            cargarCategoria();
        }
        public void cargarCategoria()
        {
            var list = (from c in db.Categoria
                        select new CategoriaTypeDTO
                        {
                           id=c.codigo,
                           nombre = c.nombre,
                           estado = c.estado
                            
                        }).ToList();

            tabla_user.ItemsSource = list;
        }
        private void btn_editar_Click(object sender, RoutedEventArgs e)
        {
            CategoriaTypeDTO categoria = ((CategoriaTypeDTO)tabla_user.SelectedItem);

            tbx_idcategoria.Text = categoria.id.ToString() ;
            tbx_nombre.Text = categoria.nombre;
            chx_estado.IsChecked = categoria.estado == 1 ? true : false;


            grilla_usuario.Visibility = Visibility.Visible;
            nuevo_usuario.Visibility = Visibility.Collapsed;

            label_accion.Text = "Editar Categoria";
        }

        private async void btn_registrar_Click(object sender, RoutedEventArgs e)
        {
            string msj = "";
            if (label_accion.Text == "Editar Categoria") 
            {
                msj= Validar2();

                if (msj == string.Empty)
                {

                    ModalMensaje ms = new ModalMensaje("¿Desea editar la categoria?", true);
                    ms.ShowDialog();
                    if (ms.DialogResult == true)
                    {
                        int idcategoria = int.Parse(tbx_idcategoria.Text);
                        Categoria categoria = db.Categoria.FirstOrDefault(x=>x.codigo== idcategoria);
                        categoria.nombre = tbx_nombre.Text;
                        categoria.estado = chx_estado.IsChecked==true?1:0;


                       int r = await db.SaveChangesAsync();
                        cargarCategoria();


                        grilla_usuario.Visibility = Visibility.Collapsed;
                        nuevo_usuario.Visibility = Visibility.Visible;
                        mensaje("Categoria actualizada");
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

                    ModalMensaje ms = new ModalMensaje("¿Desea registrar la categoria?", true);
                    ms.ShowDialog();
                    if (ms.DialogResult == true)
                    {
                        Categoria categoria = new Categoria();
                        categoria.nombre = tbx_nombre.Text;

                        categoria.estado = chx_estado.IsChecked.Value == true ? 1 : 0;

                        db.Categoria.Add(categoria);

                        await db.SaveChangesAsync();
                        cargarCategoria();
                        grilla_usuario.Visibility = Visibility.Collapsed;
                        nuevo_usuario.Visibility = Visibility.Visible;
                        mensaje("Categoria registrada");
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

        private async void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            int idcategoria = ((CategoriaTypeDTO)tabla_user.SelectedItem).id;
            var us = db.Categoria.FirstOrDefault(x => x.codigo == idcategoria);
            string msj = "";
            if ((sender as CheckBox).IsChecked==true)
            {
                ((CategoriaTypeDTO)tabla_user.SelectedItem).estado = 1;
                us.estado = 1;
                msj = "Categoria Activado";
            }
            else
            {
                ((UsuarioDTO)tabla_user.SelectedItem).habilitado = 0;
                msj = "Categoria Desactivada";
                us.estado = 0;
            }

           int r = await db.SaveChangesAsync();
            ModalMensaje ms = new ModalMensaje(msj, true,true);
            ms.ShowDialog();
        }
        private string Validar()
        {
            if (string.IsNullOrWhiteSpace(tbx_nombre.Text))
            {
                return "Debe ingresar el nombre";
            }
            return "";
        }
        public void limpiar()
        {
            tbx_nombre.Text = string.Empty;
            chx_estado.IsChecked = true;
        }

        private void nuevo_usuario_Click(object sender, RoutedEventArgs e)
        {
            grilla_usuario.Visibility = Visibility.Visible;
            nuevo_usuario.Visibility = Visibility.Collapsed;
            label_accion.Text = "Registrar cateogira";
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
            if (string.IsNullOrWhiteSpace(tbx_nombre.Text))
            {
                return "Debe ingresar el nombre";
            }
            return "";
        }


    }
}
