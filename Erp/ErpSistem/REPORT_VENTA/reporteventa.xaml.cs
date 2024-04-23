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
using ZXing;
using LiveCharts;
using LiveCharts.Wpf;
using System.Globalization;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using LiveCharts.Defaults;
using System.ComponentModel;

namespace ErpClass
{
    /// <summary>
    /// Lógica de interacción para pagecomanda.xaml
    /// </summary>
    public partial class reporteventa : Page, INotifyPropertyChanged
    {
        ModelDataBase db = new ModelDataBase();
        public Func<ChartPoint, string> PointLabel { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public  reporteventa()
        {
            InitializeComponent();
            LoadReporte();
            LoadReporteVentas();
            LoadReporteProducto();
            lb_titulo.Content = "SISTEMA DE REPORTES " + DateTime.Now.Date.Year;
            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public SeriesCollection ProductosList { get; set; }
        public string[] Labels { get; set; }
        public string _TotalDia { get; set; }
        public string TotalDia
        {

            get
            {
                return _TotalDia;
            }

            set
            {
                _TotalDia = value;
                OnPropertyChanged("TotalDia");
            }
        }
        public string _CantidadVenDia { get; set; }
        public string CantidadVenDia
        {

            get
            {
                return _CantidadVenDia;
            }

            set
            {
                _CantidadVenDia = value;
                OnPropertyChanged("CantidadVenDia");
            }
        }
        public string _MontoTotalFLETES { get; set; }
        public string MontoTotalFLETES
        {

            get
            {
                return _MontoTotalFLETES;
            }

            set
            {
                _MontoTotalFLETES = value;
                OnPropertyChanged("MontoTotalFLETES");
            }
        }
        public string _CantidadTotalFlete { get; set; }
        public string CantidadTotalFlete
        {

            get
            {
                return _CantidadTotalFlete;
            }

            set
            {
                _CantidadTotalFlete = value;
                OnPropertyChanged("CantidadTotalFlete");
            }
        }
        public Func<double, string> Formatter { get; set; }

        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartpoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries)chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
        }
        public void LoadReporte()
        {
           DateTime? fecha= txt_fecha.SelectedDate;
            List<ReporteSistemaDTO> list = new List<ReporteSistemaDTO>();

            list = db.Database.SqlQuery<ReporteSistemaDTO>("sp_atenciones_reporte_sistema(@fecha)", new MySqlParameter("@fecha", fecha)).ToList();

            var ventaTotalDia = list.FirstOrDefault(x => x.tipo == "TOTAL_VENTAS").cantidad;
            var montoTotalFlete = list.FirstOrDefault(x => x.tipo == "TOTAL DESP").cantidad;


            TotalDia = quitarDecimal(ventaTotalDia.ToString("N", new CultureInfo("es-CL")));
            CantidadTotalFlete = list.FirstOrDefault(x => x.tipo == "DESPACHO").cantidad.ToString();
            MontoTotalFLETES = quitarDecimal(montoTotalFlete.ToString("N", new CultureInfo("es-CL")));
            CantidadVenDia = list.FirstOrDefault(x => x.tipo == "VENTAS").cantidad.ToString();



        }
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        public void LoadReporteVentas()
        {
            List<ReporteVentaDTO> list = new List<ReporteVentaDTO>();
            list = db.Database.SqlQuery<ReporteVentaDTO>("sp_reporte_venta(@anio)", new MySqlParameter("@anio", DateTime.Now.Date.Year.ToString()))
            .ToList();



            Labels = new string[list.Count];
            ChartValues<long> totales = new ChartValues<long>();
            for (int i = 0; i < list.Count; i++)
            {
                Labels[i] = list[i].mes;
                totales.Add(list[i].total);

            }
            Formatter = value => value.ToString("N");

            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = DateTime.Now.Date.Year.ToString(),
                    Values = totales
                }
            };

        }
        public void LoadReporteProducto()
        {
            List<ReporteSistemaDTO> list = new List<ReporteSistemaDTO>();
            list = db.Database.SqlQuery<ReporteSistemaDTO>("sp_report_producto_mas_vendidos(@anio)", new MySqlParameter("@anio", DateTime.Now.Date.Year.ToString()))
            .ToList();

            ProductosList = new SeriesCollection();

            foreach (var item in list)
            {
                PieSeries pie = new PieSeries
                {
                    Title = item.tipo,
                    Values = new ChartValues<int> {item.cantidad},
                    DataLabels = true
                };

                ProductosList.Add(pie);
            }
            PointLabel = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);


        }
        public string quitarDecimal(string numero)
        {
            var index = numero.IndexOf(",");
            numero = numero.Substring(0, index);
            return numero;
        }

        private void btn_buscar_reporte_Click(object sender, RoutedEventArgs e)
        {
            LoadReporte();
        }
    }
}
