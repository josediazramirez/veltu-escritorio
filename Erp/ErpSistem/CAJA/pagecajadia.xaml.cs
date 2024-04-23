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
using System.Data;
using DTO;
using System.Data.Entity.Core;
using System.Drawing.Printing;
using System.Drawing;
using Model;
using System.Windows.Threading;
using System.Data.Entity;

namespace ErpClass
{
    /// <summary>
    /// Lógica de interacción para pagecomanda.xaml
    /// </summary>
    public partial class cajadia : Page
    {
        InformeController controller = new InformeController();
        ModelDataBase model = new ModelDataBase();
        public cajadia()
        {
            InitializeComponent();
            getCajaxdia(DateTime.Now.Date);

        }
        public async void getCajaxdia(DateTime fecha)
        {
            try
            {
                List<CajDTO> lista = new List<CajDTO>();
                lista = await controller.getCajaxdia(fecha);
                grid_caja.ItemsSource = lista;
            }
            catch (EntityException e)
            {
                MessageBox.Show(e.Message);
            }

        }
        public async void getCajaDet(int idcaja)
        {
            try
            {
                List<movcajdto> lista = new List<movcajdto>();
                lista = await controller.getCajaDet(idcaja);
                grid_caja_det.ItemsSource = lista;
            }
            catch (EntityException e)
            {
                MessageBox.Show(e.Message);
            }

        }

        private void btn_detalle_Click(object sender, RoutedEventArgs e)
        {
            int id = ((CajDTO)grid_caja.SelectedItem).codigo;
            getCajaDet(id);


        }

        private void btn_buscar_Click(object sender, RoutedEventArgs e)
        {
            DateTime? x_fecha = fecha.SelectedDate;
            if (x_fecha!=null)
            {
                getCajaxdia(x_fecha.Value);
            }
            else
            {
                ModalMensaje mensaje = new ModalMensaje("Debe ingresar fecha");
                mensaje.ShowDialog();
            }
            
        }
    }
}
