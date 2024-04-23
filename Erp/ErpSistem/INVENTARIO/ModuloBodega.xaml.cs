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

namespace ErpAdmin.INVENTARIO
{
    /// <summary>
    /// Lógica de interacción para ModuloUser.xaml
    /// </summary>
    public partial class ModuloBodega : Page
    {
        ModelDataBase db = new ModelDataBase();
        public ModuloBodega()
        {
            InitializeComponent();
            CargarBodegas();
            
        }
        public void CargarBodegas()
        {
            var list = (from c in db.centro
                        select new CentroDTO
                        {
                            id = c.idcentro,
                            nombre = c.nombre,
                            estado = c.estado,
                            CanSell = c.cansell,
                            autorizacion = c.autorizacion
                            
                        }).ToList();

            tabla_user.ItemsSource = list;
        }
        private void btn_editar_Click(object sender, RoutedEventArgs e)
        {
            CentroDTO bodega = ((CentroDTO)tabla_user.SelectedItem);
            if (bodega.id==1)
            {
                chx_estado.Visibility = Visibility.Collapsed;
                chx_can_sell.Visibility = Visibility.Collapsed;
            }
            else
            {
                chx_estado.Visibility = Visibility.Visible;
                chx_can_sell.Visibility = Visibility.Visible;
            }
            tbx_idbodega.Text = bodega.id.ToString();
            tbx_nombre.Text = bodega.nombre;
            chx_estado.IsChecked = bodega.estado == 1 ? true : false;
            chx_can_sell.IsChecked = bodega.CanSell == 1 ? true : false;
            chx_autoriza.IsChecked = bodega.autorizacion == 1 ? true : false; 




            grilla_usuario.Visibility = Visibility.Visible;
            nuevo_usuario.Visibility = Visibility.Collapsed;


            label_accion.Text = "Editar bodega";
        }

        private async void btn_registrar_Click(object sender, RoutedEventArgs e)
        {
            string msj = "";
            if (label_accion.Text == "Editar bodega") 
            {
                msj= Validar2();

                if (msj == string.Empty)
                {

                    ModalMensaje ms = new ModalMensaje("¿Desea editar la bodega?", true);
                    ms.ShowDialog();
                    if (ms.DialogResult == true)
                    {
                        int idbodega = int.Parse(tbx_idbodega.Text);
                        centro bodega = db.centro.FirstOrDefault(x=>x.idcentro== idbodega);

                        bodega.nombre = tbx_nombre.Text;
                        bodega.estado = chx_estado.IsChecked==true?1:0;
                        bodega.cansell = chx_can_sell.IsChecked==true?1:0;
                        bodega.autorizacion = chx_autoriza.IsChecked == true ? 1 : 0;


                       int r = await db.SaveChangesAsync();
                        CargarBodegas();


                        grilla_usuario.Visibility = Visibility.Collapsed;
                        nuevo_usuario.Visibility = Visibility.Visible;
                        mensaje("Bodega actualizada");
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

                    ModalMensaje ms = new ModalMensaje("¿Desea registrar la bodega?", true);
                    ms.ShowDialog();
                    if (ms.DialogResult == true)
                    {
                        centro bodega = new centro();
                        bodega.nombre = tbx_nombre.Text;
                        bodega.estado = chx_estado.IsChecked.Value == true ? 1 : 0;
                        bodega.cansell = chx_can_sell.IsChecked.Value == true ? 1 : 0;
                        bodega.autorizacion = chx_autoriza.IsChecked == true ? 1 : 0;

                        db.centro.Add(bodega);

                        await db.SaveChangesAsync();
                        CargarBodegas();
                        grilla_usuario.Visibility = Visibility.Collapsed;
                        nuevo_usuario.Visibility = Visibility.Visible;
                        mensaje("Bodega registrada");
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
            int idbodega = ((CentroDTO)tabla_user.SelectedItem).id;
            var us = db.centro.FirstOrDefault(x => x.idcentro == idbodega);
            string msj = "";
            if ((sender as CheckBox).IsChecked==true)
            {
                ((CentroDTO)tabla_user.SelectedItem).estado = 1;
                us.estado = 1;
                msj = "Bodega Activada";
            }
            else
            {
                ((CentroDTO)tabla_user.SelectedItem).estado = 0;
                msj = "Bodega Desactivada";
                us.estado = 0;
            }

           int r = await db.SaveChangesAsync();
            ModalMensaje ms = new ModalMensaje(msj,null,true);
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
            chx_can_sell.IsChecked = false;
        }

        private void nuevo_usuario_Click(object sender, RoutedEventArgs e)
        {
            grilla_usuario.Visibility = Visibility.Visible;
            nuevo_usuario.Visibility = Visibility.Collapsed;
            label_accion.Text = "Registrar bodega";
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


        private async void check_habilitado_Click(object sender, RoutedEventArgs e)
        {
            int idbodega = ((CentroDTO)tabla_user.SelectedItem).id;
            var us = db.centro.FirstOrDefault(x => x.idcentro == idbodega);
            string msj = "";
            if ((sender as CheckBox).IsChecked == true)
            {
                ((CentroDTO)tabla_user.SelectedItem).estado = 1;
                us.cansell = 1;
                msj = "La Bodega puede vender";
            }
            else
            {
                ((CentroDTO)tabla_user.SelectedItem).estado = 0;
                msj = "La Bodega no puede vender";
                us.cansell = 0;
            }

            int r = await db.SaveChangesAsync();
            ModalMensaje ms = new ModalMensaje(msj, null, true);
            ms.ShowDialog();
        }

        private async void check_autorizacion_Checked(object sender, RoutedEventArgs e)
        {
            int idbodega = ((CentroDTO)tabla_user.SelectedItem).id;
            var us = db.centro.FirstOrDefault(x => x.idcentro == idbodega);
            string msj = "";
            if ((sender as CheckBox).IsChecked == true)
            {
                ((CentroDTO)tabla_user.SelectedItem).autorizacion = 1;
                us.autorizacion = 1;
                msj = "Autorización habilita";
            }
            else
            {
                ((CentroDTO)tabla_user.SelectedItem).autorizacion = 0;
                msj = "Autorización no habilitada";
                us.autorizacion = 0;
            }

            int r = await db.SaveChangesAsync();
            ModalMensaje ms = new ModalMensaje(msj, null, true);
            ms.ShowDialog();
        }
    }
}
