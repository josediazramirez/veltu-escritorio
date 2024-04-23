using Controller;
using DTO;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ErpClass
{
    /// <summary>
    /// Lógica de interacción para Pagecliente.xaml
    /// </summary>
    public partial class Pagepedido : Page
    {
        readonly ModelDataBase db = new ModelDataBase();
        readonly CajaArqueoContro fn = new CajaArqueoContro();
        public string name { get; set; }
        public static int total;

        public static ObservableCollection<DetalleDTO> list_pro = new ObservableCollection<DetalleDTO>();
        public static ObservableCollection<AtencionDTO> list_ate = new ObservableCollection<AtencionDTO>();
        public Pagepedido()
        {
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
            InitializeComponent();
            InitListObser();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GetEstCaja();
        }
        async void GetEstCaja()
        {
            var caja = await fn.GetUltCaja(GlobalClass.idusuario);
            DateTime now = DateTime.Now;
            if (caja != null)
            {
                if (caja.fecha.Date == now.Date)
                {
                    if (caja.estado == "Cierre")
                    {
                        mensaje("Debe hacer arqueo de caja");
                        GlobalClass.idcaja = null;

                    }
                    else
                    {
                        GlobalClass.idcaja = caja.codigo;
                    }
                }
                else
                {
                    mensaje("Debe hacer arqueo de caja");
                    GlobalClass.idcaja = null;

                }
            }
            else
            {
                mensaje("Debe hacer arqueo de caja");
            }
        }
        public void InitListObser()
        {
            dgi_detalles.ItemsSource = list_pro;
            grilla_pedido.ItemsSource = list_ate;

        }
        private void ValAtencion(List<atencion> atencions)
        {
            List<int> LiIdAten = new List<int>();

            List<atencion> aten_ready = atencions.Where(x => x.idestado_atencion == 2 || x.idestado_atencion == 3).ToList();
            List<atencion> aten_recep = atencions.Where(x => x.idestado_atencion == 1).ToList();

            for (int p = 0; p < aten_ready.Count; p++)
            {
                var aten = list_ate.FirstOrDefault(x => x.idatencion == aten_ready[p].idatencion);
                if (aten != null)
                {
                    list_ate.Remove(aten);
                }

            }
            for (int i = 0; i < list_ate.Count; i++)
            {
                for (int p = 0; p < aten_recep.Count; p++)
                {
                    if (list_ate[i].idatencion == aten_recep[p].idatencion)
                    {
                        LiIdAten.Add(list_ate[i].idatencion);
                    }
                }
            }
            foreach (var idaten in LiIdAten)
            {
                atencions.RemoveAll(x => x.idatencion == idaten);
            }

        }
        public async void cargarAtencion()
        {
            var fecha = DateTime.Now.Date;
            List<atencion> atencions = new List<atencion>();
            using (var dba = new ModelDataBase())
            {
                atencions = await (from ate in dba.atencion
                                   where DbFunctions.TruncateTime(ate.fecha) == fecha
                                   select ate).ToListAsync();
            };

            ValAtencion(atencions);
            List<atencion> news = new List<atencion>();
            news = atencions.Where(x => x.idestado_atencion == 1).ToList();
            foreach (var item in news)
            {
                AtencionDTO detalleDTO = new AtencionDTO();

                if (item.idtipoatencion == 1)
                {
                    detalleDTO.idpedido = item.idpedido.Value;
                    detalleDTO.tip_atencion = "VENTA";
                    detalleDTO.color_atencion = "red";
                }
                else if (item.idtipoatencion == 2)
                {

                    detalleDTO.idventadevolucion = item.idventadevolucion.Value;
                    detalleDTO.tip_atencion = "DEVOLUCIÓN";
                    detalleDTO.color_atencion = "yellow";

                }
                else if (item.idtipoatencion == 3)
                {

                    detalleDTO.idpedido = item.idpedido.Value;
                    detalleDTO.idventadevolucion = item.idventadevolucion.Value;
                    detalleDTO.tip_atencion = "CAMBIO";
                    detalleDTO.color_atencion = "green";
                }
                detalleDTO.num_atencion = item.numero_atencion;
                detalleDTO.vendedor = item.vendedor;
                detalleDTO.idtipo_atencion = item.idtipoatencion;
                detalleDTO.idatencion = item.idatencion;

                list_ate.Add(detalleDTO);
            }
        }
        private void Button_atencion_Click(object sender, RoutedEventArgs e)
        {
            SelAtencion();
        }
        public void AddProList(DetalleDTO detalle)
        {
            list_pro.Add(detalle);
            total += detalle.total;
            mantenerDatos();

        }
        private int SelAtencion()
        {
            int tipo = 0;
            limpiarControles(1);
            var obj = (grilla_pedido.SelectedItem as AtencionDTO);
            if (obj != null)
            {
                lb_num_pedido.Content = "N° DE ATENCION " + obj.num_atencion;
                if (obj.idtipo_atencion == 1 || obj.idtipo_atencion == 3)
                {
                    tipo = 1;
                    var fecha = DateTime.Now.Date;
                    var pedido = (from ped in db.Pedido
                                  join det in db.Detalle_pedido
                                  on ped.codigo equals det.pedido
                                  join pro in db.Producto
                                  on det.codigo equals pro.codigo
                                  join co in db.color
                                   on pro.idcolor equals co.idcolor
                                   into colors
                                  from col in colors.DefaultIfEmpty()
                                  join mar in db.marca
                                 on pro.idmarca equals mar.idmarca
                                 into mars
                                  from marc in mars.DefaultIfEmpty()
                                  join inv in db.inventario
                                  on det.idinventario equals inv.idinventario
                                  into med 
                                  from meds in med.DefaultIfEmpty()
                                  where ped.codigo == obj.idpedido
                                  select new DetalleDTO
                                  {
                                      code_pedido = ped.codigo,
                                      codigo = pro.codigo,
                                      producto = pro.nombre,
                                      precio = pro.precio.Value,
                                      Cantidad = det.cantidad,
                                      code_detalle = ped.mp_id.Value,
                                      idinventario = det.idinventario,
                                      marca = marc.nombre,
                                      color = col.nombre,
                                      precio_desc = det.precio_desc,
                                      idmedida = meds.idmedida,
                                      total = det.total

                                  }).ToList();
                    GlobalClass.idpedido = pedido[0].code_pedido;

                    foreach (var item in pedido)
                    {
                        DetalleDTO detalle = new DetalleDTO();

                        detalle.idmedida = item.idmedida;
                        detalle.codigo = item.codigo;
                        detalle.producto = item.producto + " " + item.color + " " + item.marca;
                        detalle.precio = item.precio;
                        detalle.Cantidad = item.Cantidad;
                        detalle.total = item.total;
                        detalle.idinventario = item.idinventario;
                        detalle.centro = item.centro;
                        detalle.precio_desc = item.precio_desc != null ? item.precio_desc : 0;


                        AddProList(detalle);
                    }
                    if (obj.idtipo_atencion == 3)
                    {
                        var obje = db.venta_devolucion.FirstOrDefault(x => x.idventadevolucion == obj.idventadevolucion.Value);
                        GlobalClass.saldo_devolucion = obje.total_devolucion;
                        GlobalClass.idventadevolucion = obj.idventadevolucion.Value;
                    }

                    //mensaje("ATENCIÓN N° " + obj.num_atencion);
                }
                if (obj.idtipo_atencion == 2)
                {
                    tipo = 2;
                    if (GlobalClass.idcaja == null)
                    {
                        mensaje("Debe hacer arqueo de caja");
                    }
                    else
                    {


                        var devolu = (from ven_dev in db.venta_devolucion
                                      join dev_pro in db.devo_producto
                                      on ven_dev.idventadevolucion equals dev_pro.idventadevolucion
                                      join pro in db.Producto
                                      on dev_pro.idproducto equals pro.codigo
                                      where ven_dev.idventadevolucion == obj.idventadevolucion
                                      select new DetalleDTO
                                      {
                                          code_pedido = ven_dev.idventadevolucion,
                                          idinventario = dev_pro.idinventario,
                                          codigo = pro.codigo,
                                          producto = pro.nombre,
                                          precio = dev_pro.precio,
                                          Cantidad = dev_pro.cantidad,
                                          total = dev_pro.total

                                      }).ToList();

                        int total = 0;
                        foreach (var item in devolu)
                        {
                            DetalleDTO detalle = new DetalleDTO();

                            detalle.codigo = item.codigo;
                            detalle.producto = item.producto;
                            detalle.precio = item.precio;
                            detalle.Cantidad = item.Cantidad;
                            detalle.total = item.total;
                            total = total + detalle.total;
                            detalle.idinventario = item.idinventario;
                        }

                        mantenerDatos();
                        ModalDevolucion modal = new ModalDevolucion(devolu, total, devolu[0].code_pedido);
                        bool? resul = modal.ShowDialog();
                        if (resul == true)
                        {
                            var objAte = db.atencion.FirstOrDefault(x => x.idatencion == obj.idatencion);
                            objAte.idestado_atencion = 2;
                            db.SaveChanges();

                            AtencionDTO objate = list_ate.FirstOrDefault(x => x.idatencion == objAte.idatencion);
                            list_ate.Remove(objate);
                        }
                        limpiarControles();
                    }
                }
            }
            return tipo;
        }
        private void btn_venta_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalClass.idcaja == null)
            {
                mensaje("Debe hacer arqueo de caja");
            }
            else
            {
                GetVenta();
            }
        }
        public async void GetVenta()
        {
            if (GlobalClass.idcaja == null)
            {
                mensaje("Debe hacer arqueo de caja");
            }
            else
            {


                if (grilla_pedido.SelectedValue != null)
                {
                    var obj = (grilla_pedido.SelectedItem as AtencionDTO);

                    if (dgi_detalles.Items.Count == 0)
                    {
                        mensaje("Debe seleccionar atención");
                    }
                    else
                    {
                        mov_caja caja = db.mov_caja.FirstOrDefault(x => x.idpedido == GlobalClass.idpedido);
                        if (caja == null)
                        {
                            GlobalClass.productos.Clear();

                            foreach (DetalleDTO item in dgi_detalles.Items)
                            {
                                GlobalClass.productos.Add(item);
                            }
                            GlobalClass.total = total;

                            ModalVenta modal = new ModalVenta();
                            modal.ShowDialog();
                            if (GlobalClass.estado == 1)
                            {
                                var objAte = await db.atencion.FirstOrDefaultAsync(x => x.idatencion == obj.idatencion);
                                objAte.idestado_atencion = 2;
                                int ven = await db.SaveChangesAsync();
                                if (ven > 0)
                                {
                                    AtencionDTO objate = list_ate.FirstOrDefault(x => x.idatencion == objAte.idatencion);
                                    list_ate.Remove(objate);
                                    limpiarControles();
                                }

                            }
                        }
                        else
                        {
                            mensaje("No existe atención");
                        }

                    }

                }
                else
                {
                    mensaje("No existe atención");
                }
            }
        }
        private void limpiarControles(int? mod = null)
        {
            GlobalClass.saldo_devolucion = null;
            GlobalClass.idventadevolucion = null;
            GlobalClass.estado = 0;
            total = 0;
            mantenerDatos();
            list_pro.Clear();
            lb_num_pedido.Content = string.Empty;
        }
        private async void btn_cancelar_atencion_Click(object sender, RoutedEventArgs e)
        {
            HabilitarProceso hb = new HabilitarProceso("Autorización eliminar atención",2);
            bool? hab = hb.ShowDialog();
            if (hab == true)
            {
                var obj = (grilla_pedido.SelectedItem as AtencionDTO);
                if (obj != null)
                {
                    ModalMensaje modal = new ModalMensaje("¿Desea cancelar " + obj.tip_atencion + " N° " + obj.num_atencion + "?", true);
                    bool? resp = modal.ShowDialog();
                    if (resp == true)
                    {
                        GlobalClass.saldo_devolucion = null;
                        GlobalClass.idventadevolucion = null;
                        total = 0;
                        list_pro.Clear();
                        atencion ate = await db.atencion.FirstOrDefaultAsync(x => x.idatencion == obj.idatencion);
                        ate.idestado_atencion = 3;
                        int resulAte = await db.SaveChangesAsync();
                        if (resulAte > 0)
                        {
                            if (obj.idtipo_atencion == 1)
                            {
                                Pedido ped = db.Pedido.FirstOrDefault(x => x.codigo == obj.idpedido);
                                ped.idestado = 3;
                                int resulPed = await db.SaveChangesAsync();
                                if (resulPed > 0)
                                {
                                    AtencionDTO objate = list_ate.FirstOrDefault(x => x.idatencion == ate.idatencion);
                                    list_ate.Remove(objate);
                                    limpiarControles();
                                    mensaje("Atención cancelada");

                                }
                            }
                        }
                    }
                }
            }
            else
            {
                ModalMensaje modal = new ModalMensaje("No tiene acceso", null, true);
                modal.ShowDialog();
            }
        }
        public void mantenerDatos()
        {
            lbl_total.Content = "Total $" + String.Format("{0:N0}", total);
        }
        private bool? mensaje(string msj)
        {
            ModalMensaje modal = new ModalMensaje(msj);
            return modal.ShowDialog();
        }
        public async void cargarAtencion(int numeroAte)
        {
            var fecha = DateTime.Now.Date;
            List<atencion> atencions = new List<atencion>();
            using (var dba = new ModelDataBase())
            {
                atencions = await (from ate in dba.atencion
                                   where DbFunctions.TruncateTime(ate.fecha) == fecha
                                   && ate.numero_atencion == numeroAte
                                   select ate).ToListAsync();
            };

            ValAtencion(atencions);
            List<atencion> news = new List<atencion>();
            news = atencions.Where(x => x.idestado_atencion == 1).ToList();
            foreach (var item in news)
            {
                AtencionDTO detalleDTO = new AtencionDTO();

                if (item.idtipoatencion == 1)
                {
                    detalleDTO.idpedido = item.idpedido.Value;
                    detalleDTO.tip_atencion = "VENTA";
                    detalleDTO.color_atencion = "red";
                }
                else if (item.idtipoatencion == 2)
                {

                    detalleDTO.idventadevolucion = item.idventadevolucion.Value;
                    detalleDTO.tip_atencion = "DEVOLUCIÓN";
                    detalleDTO.color_atencion = "yellow";

                }
                else if (item.idtipoatencion == 3)
                {

                    detalleDTO.idpedido = item.idpedido.Value;
                    detalleDTO.idventadevolucion = item.idventadevolucion.Value;
                    detalleDTO.tip_atencion = "CAMBIO";
                    detalleDTO.color_atencion = "green";
                }
                detalleDTO.num_atencion = item.numero_atencion;
                detalleDTO.vendedor = item.vendedor;
                detalleDTO.idtipo_atencion = item.idtipoatencion;
                detalleDTO.idatencion = item.idatencion;

                list_ate.Add(detalleDTO);
            }
        }
        private void btn_update_aten_Click(object sender, RoutedEventArgs e)
        {
            list_ate.Clear();
            grilla_pedido.ItemsSource = list_ate;
            BuscarAtencion buscarAtencion = new BuscarAtencion();
            var result = buscarAtencion.ShowDialog();
            if (result==true)
            {
                cargarAtencion(buscarAtencion._idatencion);
                grilla_pedido.SelectedIndex = 0;
                int tipo = SelAtencion();
                if (GlobalClass.idcaja == null)
                {
                    mensaje("Debe hacer arqueo de caja");
                }
                else
                {
                    if (tipo!=2)
                    {
                        GetVenta();
                    }
                }
            }
        }
    }
}
