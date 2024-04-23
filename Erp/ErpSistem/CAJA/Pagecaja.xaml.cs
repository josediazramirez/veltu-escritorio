using Controller;
using DTO;
using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ErpClass
{
    /// <summary>
    /// Lógica de interacción para Pagecaja.xaml
    /// </summary>
    public partial class Pagecaja : Page
    {

        readonly CajaArqueoContro fn = new CajaArqueoContro();
        public Pagecaja()
        {
            Loaded += MainWindow_Loaded;
            InitializeComponent();
            loadtipoPago();
        }
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GetUltimaCaja();
            GetMovIngEgre();
        }
        private void loadtipoPago()
        {
            List<MedioPago> medioPago = new List<MedioPago>();

            medioPago.Add(new MedioPago { mp_id = 1, mp_nombre = "EFECTIVO" });
            medioPago.Add(new MedioPago { mp_id = 2, mp_nombre = "DEBITO" });
            medioPago.Add(new MedioPago { mp_id = 3, mp_nombre = "TRANSFERENCIA" });
            cbx_pago.ItemsSource = medioPago;
        }
        private async void GetMovIngEgre()
        {
            gif_sale.Visibility = Visibility.Visible;
            try
            {
                List<arqueoDTO> li_ing = await fn.GetIngresos(GlobalClass.idcaja.Value);
                List<arqueoDTO> li_egre= await fn.GetEgresos(GlobalClass.idcaja.Value);
                List<MovCajaDTO> li_movIngEgre= await fn.GetMovIngEgre(GlobalClass.idcaja.Value);
                tabla_ingresos.ItemsSource = li_ing;
                tabla_egresos.ItemsSource = li_egre;
                tabla_mov_ing_egre.ItemsSource = li_movIngEgre;
                GetSaldoTotalEfec(li_ing, li_egre);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            gif_sale.Visibility = Visibility.Collapsed;
        }
        private void GetSaldoTotalEfec(List<arqueoDTO> ent, List<arqueoDTO> sal)
        {
            decimal total_ent = ent.Where(x => x.mp_id == 1).Sum(t => t.total_ent);
            decimal total_sal = sal.Sum(t => t.total_sal);
            int totalEfectivo = (int)(total_ent - total_sal);

            lb_sal_sistema.Content = GetNumberFormat(totalEfectivo);

            int total_caja = getNumber(lb_sal_ini) + totalEfectivo;
            lb_total_caja.Content = GetNumberFormat(total_caja);

            decimal total_ing = ent.Where(x => x.mp_id != 6).Sum(t => t.total_ent);
            decimal total_egre = sal.Sum(t => t.total_sal);

            int saldo_total = (int)(total_ing - total_egre);
            lb_saldo_total.Content = GetNumberFormat(saldo_total);
        }
        private int getNumber(Label text)
        {
            string valor = text.Content.ToString().Replace(".", "").ToString();
            int num = 0;
            if (int.TryParse(valor, out num))
            {
                return num;
            }
            return num;
        }
        private async void GetUltimaCaja()
        {
            DateTime now = DateTime.Now;
            var caja = await fn.GetUltCaja(GlobalClass.idusuario);
            GetDatosCaja(caja);

            if (caja != null)
            {
                if (caja.fecha.Date == now.Date)
                {
                    if (caja.estado == "Inicio")
                    {
                        btn_apertura.Content = "CIERRE DE CAJA";
                        btn_apertura.Tag = "Cierre";
                    }
                    else
                    {
                        btn_apertura.Content = "APERTURA DE CAJA";
                        btn_apertura.Tag = "Inicio";
                    }
                }
                else
                {
                    if (caja.estado == "Inicio")
                    {
                        btn_apertura.Content = "DEBE CERRAR CAJA";
                        btn_apertura.Tag = "debecerrar";
                    }
                    else
                    {
                        btn_apertura.Content = "APERTURA DE CAJA";
                        btn_apertura.Tag = "Inicio";
                    }
                }
            }
        }
        private void GetDatosCaja(CajaDTO caja)
        {
            GlobalClass.idcaja = caja.codigo;
            lb_cajero.Content = caja.user_nombre + " " + caja.user_apellido;
            lb_cod_caja.Content = caja.codigo;
            lb_fecha_inicio.Content = caja.hora_inicio.ToString();
            lb_fecha_termino.Content = caja.hora_termino.ToString();

            lb_sal_ini.Content = GetNumberFormat(caja.saldo_inicial);
            lb_saldo_diferencia.Content = GetNumberFormat(caja.saldo_diferencia);
            lb_saldo_cierre.Content = GetNumberFormat(caja.saldo_cierre);
        }
        private string GetNumberFormat(int? valor)
        {
            if (valor != null)
            {
                return String.Format("{0:N0}", valor);
            }
            else
            {
                return "0";
            }
        }
        private async void Btn_apertura_Click(object sender, RoutedEventArgs e)
        {
            ModalAperturaCierre modal = new ModalAperturaCierre();
            bool? resul = modal.ShowDialog();
            if (resul == true)
            {
                if (btn_apertura.Tag.ToString()=="Inicio")
                {
                   var caja = await fn.CreateUpdateNewCaja(GlobalClass.idcaja.Value, GlobalClass.idusuario,"Inicio", modal.total, 0, 0, 0);
                    GlobalClass.idcaja = caja.codigo;
                    btn_apertura.Content = "CIERRE DE CAJA";
                    btn_apertura.Tag = "Cerrar";
                    ModalMensaje modal1 = new ModalMensaje("APERTURA DE CAJA REALIZADO");
                    modal1.ShowDialog();
                
                    clearControlCierre();

                    lb_cod_caja.Content = caja.codigo;
                    lb_fecha_inicio.Content = caja.hora_inicio;
                    lb_sal_ini.Content = GetNumberFormat(modal.total);
                    lb_total_caja.Content = GetNumberFormat(modal.total);
                    lb_saldo_total.Content = GetNumberFormat(0);

                    printTicketApertura("APERTURA DE CAJA", modal.total.ToString(), caja.hora_inicio.ToString());

                }
                else
                {
                    int saldo_esperado = getNumber(lb_total_caja);
                    int saldo_hay = modal.total;
                    int dife = saldo_esperado-saldo_hay;
                    int saldo_diferencia = dife;

                    var ca = await fn.CreateUpdateNewCaja(GlobalClass.idcaja.Value, GlobalClass.idusuario,
                        "Cierre",
                        modal.total,
                        saldo_esperado,
                        saldo_hay,
                        saldo_diferencia);

                    lb_saldo_cierre.Content = GetNumberFormat(saldo_hay);
                    lb_saldo_diferencia.Content = GetNumberFormat(dife);
                    lb_fecha_termino.Content = ca.hora_termino;
                    btn_apertura.Content = "APERTURA DE CAJA";
                    btn_apertura.Tag = "Inicio";

                    printTicketCierre("ARQUEO DE CAJA", ca.hora_inicio.ToString(), ca.hora_termino.ToString());

                    ModalMensaje modal1 = new ModalMensaje("CIERRE DE CAJA REALIZADO");
                    modal1.ShowDialog();
                }
            }
        }
        public string GetText(string titulo,string saldo_inicial, string fecha_hora_inicio)
        {


            StringBuilder textoComanda = new StringBuilder();
            string str = new string(' ', 5);
            string str2 = new string(' ', 5);
            string linea = new string('_', 25);
            string com = new string(' ', 2);
            string space = new string(' ', 8);

            textoComanda.AppendLine(str2 + linea + str2);
            textoComanda.AppendLine(str2 + titulo + str2);
            textoComanda.AppendLine(str2 + linea + str2);
            textoComanda.AppendLine(str2 + "SALDO INICIAL" + str2);
            textoComanda.AppendLine(str2 + "$"+saldo_inicial + str2);
            textoComanda.AppendLine(str2 + linea + str2);
            textoComanda.AppendLine(str2 + "FECHA Y HORA DE INICIO" + str2);
            textoComanda.AppendLine(str2 + fecha_hora_inicio + str2);
            textoComanda.AppendLine(str2 + linea + str2);



            return textoComanda.ToString();
        }
        public async Task<string> GetTextCierre(string titulo, string fecha_hora_inicio, string fecha_hora_termino)
        {


            StringBuilder textoComanda = new StringBuilder();
            string str = new string(' ', 5);
            string str2 = new string(' ', 5);
            string linea = new string('_', 25);
            string com = new string(' ', 2);
            string space = new string(' ', 8);

            textoComanda.AppendLine(str2 + linea + str2);
            textoComanda.AppendLine(str2 + titulo + str2);
            textoComanda.AppendLine(str2 + linea + str2);
            textoComanda.AppendLine(str2 + "SALDO INICIAL" + str2);
            textoComanda.AppendLine(str2 + "$" + lb_sal_ini.Content + str2);
            textoComanda.AppendLine(str2 + linea + str2);
            textoComanda.AppendLine(str2 + "SALDO DIFERENCIA" + str2);
            textoComanda.AppendLine(str2 + "$" + lb_saldo_diferencia.Content + str2);
            textoComanda.AppendLine(str2 + linea + str2);
            textoComanda.AppendLine(str2 + "SALDO TOTAL CIERRE" + str2);
            textoComanda.AppendLine(str2 + "$" + lb_total_caja.Content + str2);
            List<arqueoDTO> li_ing = await fn.GetIngresos(GlobalClass.idcaja.Value);
            foreach (var item in li_ing)
            {
                textoComanda.AppendLine(str2 + linea + str2);
                textoComanda.AppendLine(str2 + item.movimiento + str2);
                textoComanda.AppendLine(str2 + "$" + item.total_ent + str2);
            }
            List<arqueoDTO> li_eng = await fn.GetEgresos(GlobalClass.idcaja.Value);
            foreach (var item in li_eng)
            {
                textoComanda.AppendLine(str2 + linea + str2);
                textoComanda.AppendLine(str2 + item.movimiento + str2);
                textoComanda.AppendLine(str2 + "$" + item.total_sal + str2);
            }
            textoComanda.AppendLine(str2 + linea + str2);
            textoComanda.AppendLine(str2 + "FECHA Y HORA DE INICIO" + str2);
            textoComanda.AppendLine(str2 + fecha_hora_inicio + str2);
            textoComanda.AppendLine(str2 + linea + str2);
            textoComanda.AppendLine(str2 + "FECHA Y HORA DE TERMINO" + str2);
            textoComanda.AppendLine(str2 + fecha_hora_termino + str2);



            return textoComanda.ToString();
        }
        private async void printTicketCierre(string titulo, string saldo_inicial, string fecha_hora_inicio)
        {
            string ticket = await GetTextCierre(titulo, saldo_inicial, fecha_hora_inicio);
            string[] lines = ticket.Split(Environment.NewLine.ToCharArray());

            int lineas = (lines.Length * 20) + 300;

            PrintDocument p = new PrintDocument();
            PrinterSettings settings = new PrinterSettings();
            PaperSize paperSize = new PaperSize("Ticket", 300, lineas);
            settings.DefaultPageSettings.PaperSize = paperSize;

            p.PrinterSettings = settings;
            p.PrintPage += delegate (object sender1, PrintPageEventArgs e1)
            {
                Font font = new Font("Arial Bold", 10);
                e1.Graphics.DrawString(ticket, font, new SolidBrush(System.Drawing.Color.Black), 0, 0);


            };
            try
            {

                p.PrintController = new StandardPrintController();
                p.Print();
            }
            catch (Exception ex)
            {
                throw new Exception("Problema en la impresión", ex);
            }
        }
        private void printTicketApertura(string titulo, string saldo_inicial, string fecha_hora_inicio)
        {
            string ticket = GetText(titulo, saldo_inicial, fecha_hora_inicio);
            string[] lines = ticket.Split(Environment.NewLine.ToCharArray());

            int lineas = (lines.Length * 20) + 200;

            PrintDocument p = new PrintDocument();
            PrinterSettings settings = new PrinterSettings();
            PaperSize paperSize = new PaperSize("Ticket", 300, lineas);
            settings.DefaultPageSettings.PaperSize = paperSize;

            p.PrinterSettings = settings;
            p.PrintPage += delegate (object sender1, PrintPageEventArgs e1)
            {
                Font font = new Font("Arial Bold", 10);
                e1.Graphics.DrawString(ticket, font, new SolidBrush(System.Drawing.Color.Black),0, 0);


            };
            try
            {

                p.PrintController = new StandardPrintController();
                p.Print();
            }
            catch (Exception ex)
            {
                throw new Exception("Problema en la impresión", ex);
            }
        }
        public string GetTexMov(string mov, string total, string fecha, string descr)
        {


            StringBuilder textoComanda = new StringBuilder();
            string str = new string(' ', 5);
            string str2 = new string(' ', 5);
            string linea = new string('_', 25);
            string com = new string(' ', 2);
            string space = new string(' ', 8);

            textoComanda.AppendLine(str2 + linea + str2);
            textoComanda.AppendLine(str2 + mov + str2);
            textoComanda.AppendLine(str2 + linea + str2);
            textoComanda.AppendLine(str2 + "COMENTARIO" + str2);
            textoComanda.AppendLine(str2 + descr + str2);
            textoComanda.AppendLine(str2 + linea + str2);
            textoComanda.AppendLine(str2 + "TOTAL" + str2);
            textoComanda.AppendLine(str2 + "$" + total + str2);
            textoComanda.AppendLine(str2 + linea + str2);
            textoComanda.AppendLine(str2 + "FECHA Y HORA " + str2);
            textoComanda.AppendLine(str2 + fecha + str2);
            textoComanda.AppendLine(str2 + linea + str2);



            return textoComanda.ToString();
        }
        private void printTicketMov(string mov, string total,string fecha,string descr)
        {
            string ticket = GetTexMov(mov,total,fecha,descr);
            string[] lines = ticket.Split(Environment.NewLine.ToCharArray());

            int lineas = (lines.Length * 20) + 300;

            PrintDocument p = new PrintDocument();
            PrinterSettings settings = new PrinterSettings();
            PaperSize paperSize = new PaperSize("Ticket", 300, lineas);
            settings.DefaultPageSettings.PaperSize = paperSize;

            p.PrinterSettings = settings;
            p.PrintPage += delegate (object sender1, PrintPageEventArgs e1)
            {
                Font font = new Font("Arial Bold", 10);
                e1.Graphics.DrawString(ticket, font, new SolidBrush(System.Drawing.Color.Black), 0, 0);


            };
            try
            {

                p.PrintController = new StandardPrintController();
                p.Print();
            }
            catch (Exception ex)
            {
                throw new Exception("Problema en la impresión", ex);
            }
        }
        private void clearControlCierre()
        {
            lb_fecha_termino.Content = string.Empty;
            tabla_ingresos.ItemsSource = null;
            tabla_egresos.ItemsSource = null;
            tabla_mov_ing_egre.ItemsSource = null;
            lb_sal_sistema.Content = GetNumberFormat(0);
            lb_saldo_cierre.Content = GetNumberFormat(0);
            lb_saldo_diferencia.Content = GetNumberFormat(0);

        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private async void Btn_acep_mov_Click(object sender, RoutedEventArgs e)
        {
            if (btn_apertura.Tag.ToString() == "debecerrar" || btn_apertura.Tag.ToString() == "Inicio")
            {
                ModalMensaje modal = new ModalMensaje("Debe hacer arqueo de caja");
                modal.ShowDialog();
            }
            else
            {
                string val = valForminSal();
                if (val == null)
                {
                    string desc = txt_descripcion.Text;
                    int idtipo = 0;
                    int total = int.Parse(txt_total.Text);
                    string tipo = "";
                    if (check_ing.IsChecked == true)
                    {
                        idtipo = 4;
                        tipo = "INGRESO";
                    }
                    else if (check_sal.IsChecked == true)
                    {
                        idtipo = 5;
                        tipo = "EGRESO";
                    }
                    int result = await fn.SaveMov(GlobalClass.idcaja.Value, idtipo, desc, total,int.Parse(cbx_pago.SelectedValue.ToString()));
                    if (result > 0)
                    {

                        clearControlIngSal();
                        GetMovIngEgre();
                        ModalMensaje modal = new ModalMensaje("MOVIMIENTO GUARDADO");
                        modal.ShowDialog();
                        printTicketMov(tipo, total.ToString(), DateTime.Now.ToString(), desc);

                    }
                }
                else
                {
                    ModalMensaje modalMensaje = new ModalMensaje(val);
                    modalMensaje.ShowDialog();
                }
            }
        }
        private string valForminSal()
        {
            string res = null;
            if (check_ing.IsChecked==null && check_sal.IsChecked==null)
            {
                res = "Debe seleccionar el tipo movimiento";
            }
            else if (string.IsNullOrWhiteSpace(txt_descripcion.Text))
            {
                res = "Debe ingresar descripción";

            }
            else if (string.IsNullOrWhiteSpace(txt_total.Text))
            {
                res = "Debe ingresar el total";
            }
            return res;
        }
        private void clearControlIngSal()
        {
            check_ing.IsChecked = null;
            check_sal.IsChecked = null;
            txt_descripcion.Text = "";
            txt_total.Text = "";

        }
        private void Btn_canc_mov_Click(object sender, RoutedEventArgs e)
        {
            clearControlIngSal();
        }

        private async void bt_del_mov_Click(object sender, RoutedEventArgs e)
        {
            if (btn_apertura.Tag.ToString() == "debecerrar" || btn_apertura.Tag.ToString() == "Inicio")
            {
                ModalMensaje modal = new ModalMensaje("Debe hacer arqueo de caja");
                modal.ShowDialog();
            }
            else
            {

                ModalMensaje mensaje = new ModalMensaje("¿Desea eliminar el movimiento?", true);
                bool? rest = mensaje.ShowDialog();
                if (rest == true)
                {
                    int idmovcaja = (tabla_mov_ing_egre.SelectedItem as MovCajaDTO).idmovcaja;
                    int resul = await fn.DeleteMov(idmovcaja);
                    if (resul > 0)
                    {
                        GetMovIngEgre();
                        ModalMensaje mo = new ModalMensaje("MOVIMIENTO ELMINADO", null, true);
                        mo.ShowDialog();
                    }
                }
            }
        }
    }
}
