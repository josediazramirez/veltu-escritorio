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
using DTO;
using Controller;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Model;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.Entity.Core;
using System.Drawing;
using System.Drawing.Printing;
using System.Text.RegularExpressions;
using ZXing;

namespace ErpClass
{
    /// <summary>
    /// Lógica de interacción para Pagecliente.xaml
    /// </summary>
    public partial class modal_cambio : Page
    {
        InformeController controllers = new InformeController();
        public string name { get; set; }
        public string id { get; set; }
        ModelDataBase P = new ModelDataBase();


        public static ObservableCollection<DetalleDTO> detCambio = new ObservableCollection<DetalleDTO>();
        public static ObservableCollection<DetalleDTO> detDevol = new ObservableCollection<DetalleDTO>();
        public static ObservableCollection<DetalleDTO> productos = new ObservableCollection<DetalleDTO>();

        public static int saldo_total = 0;
        public modal_cambio()
        {

            InitializeComponent();
            cargarPedido();
            detCambio.Clear();
            detDevol.Clear();
            productos.Clear();
            saldo_total = 0;
        }

        private void cargarPedido()
        {
            dgi_cambios.ItemsSource = detCambio;
            dgi_devolucion.ItemsSource = detDevol;

        }
        private void txt_buscar_KeyUp(object sender, KeyEventArgs e)
        {
            
            if (e.Key == Key.Enter)
            {
                detCambio.Clear();
                detDevol.Clear();
                if (txt_buscar_boleta.Text != null)
                {
                    long num = 0;
                    if (long.TryParse(txt_buscar_boleta.Text, out num))
                    {
                        cargarPedido();
                        detCambio.Clear();
                        detDevol.Clear();
                        saldo_total = 0;

                        mantenerDatos();

                        long cod = long.Parse(txt_buscar_boleta.Text);

                        var productos = (from ped in P.Pedido
                                         join det in P.Detalle_pedido

                                         on ped.codigo equals det.pedido
                                         join inv in P.inventario
                                         on det.idinventario equals inv.idinventario

                                         join pro in P.Producto
                                         on det.codigo equals pro.codigo
                                         
                                         where ped.codigo == cod && ped.idestado == 2
                                         select new DetalleDTO
                                         {
                                             codigo = det.codigo,
                                             producto = pro.nombre,
                                             Cantidad = inv.idmedida==1? det.cantidad/1000:det.cantidad,
                                             precio = pro.precio.Value,
                                             Precio_desc = inv.idmedida == 1 ? det.precio_desc.Value: det.precio_desc.Value/det.cantidad,
                                             total = pro.precio.Value-det.precio_desc.Value,
                                             subtotal = pro.precio.Value,
                                             idinventario = det.idinventario,
                                             idmedida = inv.idmedida

                                         }).ToList();
                        var veDevuel = P.venta_devolucion.OrderByDescending(x => x.idventadevolucion).Where(x => x.idpedido == cod && x.idestadodevolucion == 2).ToList();
                        if (veDevuel.Count > 0)
                        {

                            foreach (var item in veDevuel)
                            {

                                var listProDev = P.devo_producto.Where(x => x.idventadevolucion == item.idventadevolucion).ToList();
                                foreach (var pro in productos)
                                {
                                    var sum = listProDev.Where(x => x.idproducto == pro.codigo).Sum(x => x.cantidad);
                                    pro.CantidadDispo = pro.CantidadDispo + sum;

                                }
                            }
                            foreach (var pro in productos)
                            {
                                pro.CantidadDispo = pro.Cantidad - pro.CantidadDispo;
                            }
                        }
                        else
                        {
                            foreach (var pro in productos)
                            {
                                pro.CantidadDispo = pro.Cantidad;
                            }
                        }

                        if (productos.Count == 0)
                        {
                            mensaje("La boleta no existe");
                        }
                        else
                        {
                            foreach (var detalle in productos)
                            {
                                AgregarDetalleCambio(detalle);
                            }
                        }
                    }
                    else
                    {
                        mensaje("Debe ingresar el codigo");
                    }


                }
            }
        }
        public void AgregarDetalleCambio(DetalleDTO detalle)
        {
            var x = detCambio.FirstOrDefault(ob => ob.codigo == detalle.codigo);
            if (x != null)
            {
                x.Cantidad++;
                int desc = detalle.Precio_desc == null ? 0: detalle.Precio_desc.Value;
                x.total = (x.Cantidad * x.precio)- desc;
            }
            else
            {
                int desc = detalle.Precio_desc == null ? 0 : detalle.Precio_desc.Value;
                detalle.total = detalle.idmedida==1? (detalle.precio * detalle.Cantidad) - desc:(detalle.precio * detalle.Cantidad)-(desc * detalle.Cantidad);
                detCambio.Add(detalle);
            }

        }
        public void AgregarDetalleDevol(DetalleDTO detalle, int cantidad)
        {
            var x = detDevol.FirstOrDefault(ob => ob.codigo == detalle.codigo);
            if (x != null)
            {
                if (detalle.idmedida==1)
                {
                    int desc = detalle.Precio_desc == null ? 0 : detalle.Precio_desc.Value;
                    x.Cantidad = x.Cantidad + cantidad;
                    x.total = (x.Cantidad * x.precio) - desc;
                }
                else
                {
                    int desc = detalle.Precio_desc == null ? 0 : detalle.Precio_desc.Value;
                    x.Cantidad = x.Cantidad + cantidad;
                    x.total = (x.Cantidad * x.precio) - desc * x.Cantidad;
                }

            }
            else
            {
                if (detalle.idmedida==1)
                {
                    int desc = detalle.Precio_desc == null ? 0 : detalle.Precio_desc.Value;
                    detalle.total = (detalle.Cantidad * detalle.precio) - desc;
                    detDevol.Add(detalle);
                }
                else
                {
                    int desc = detalle.Precio_desc == null ? 0 : detalle.Precio_desc.Value;
                    detalle.total = (detalle.Cantidad * detalle.precio) - (desc * detalle.Cantidad);
                    detDevol.Add(detalle);
                }
            }
            int descu = detalle.Precio_desc == null ? 0 : detalle.Precio_desc.Value;
            if (detalle.idmedida==1)
            {
                saldo_total = saldo_total + (detalle.precio * cantidad) -descu;
            }
            else
            {
                saldo_total = saldo_total + (detalle.precio * cantidad) - (descu * cantidad);
            }
            mantenerDatos();

        }

        private void btn_devolver_Click(object sender, RoutedEventArgs e)
        {
            var objDet = ((DetalleDTO)dgi_cambios.SelectedItem);
            if (objDet.CantidadDispo == 0)
            {
                ModalMensaje modal1 = new ModalMensaje("No hay cantidad para devolución");
                modal1.ShowDialog();
            }
            else
            {
                if (objDet.CantidadDispo == 1)
                {
                    objDet.CantidadDispo = objDet.CantidadDispo - 1;
                    objDet.total = objDet.precio * objDet.CantidadDispo;
                    DetalleDTO detalle = new DetalleDTO();
                    detalle.codigo = objDet.codigo;
                    detalle.Cantidad = 1;
                    detalle.producto = objDet.producto;
                    detalle.precio = objDet.precio;
                    detalle.subtotal = objDet.precio;
                    detalle.total = objDet.precio * 1;
                    detalle.Precio_desc = objDet.Precio_desc;
                    detalle.idinventario= objDet.idinventario;
                    AgregarDetalleDevol(detalle, 1);
                }
                else
                {
                    if (objDet.idmedida==1)
                    {
                        int desc = objDet.Precio_desc == null ? 0 : objDet.Precio_desc.Value;

                        objDet.CantidadDispo = objDet.CantidadDispo - objDet.Cantidad;

                        objDet.total = objDet.precio * objDet.CantidadDispo;

                        DetalleDTO detalle = new DetalleDTO();
                        detalle.codigo = objDet.codigo;
                        detalle.Cantidad = objDet.Cantidad;
                        detalle.producto = objDet.producto;
                        detalle.precio = objDet.precio;
                        detalle.subtotal = objDet.precio;
                        detalle.idmedida= objDet.idmedida;
                        detalle.idinventario = objDet.idinventario;

                        detalle.total = objDet.precio * detalle.cantidad - desc;
                        detalle.Precio_desc = objDet.Precio_desc;
                        AgregarDetalleDevol(detalle, objDet.Cantidad);
                    }
                    else
                    {
                        modal_cant_devol modal_Cant_Devol = new modal_cant_devol(objDet.Cantidad);
                        var modal = modal_Cant_Devol.ShowDialog();
                        if (modal == true)
                        {
                            int desc = objDet.Precio_desc == null ? 0 : objDet.Precio_desc.Value;

                            objDet.CantidadDispo = objDet.CantidadDispo - modal_Cant_Devol.cantidad;
                            objDet.total = objDet.precio * objDet.CantidadDispo - desc * objDet.CantidadDispo;
                            DetalleDTO detalle = new DetalleDTO();
                            detalle.codigo = objDet.codigo;
                            detalle.Cantidad = modal_Cant_Devol.cantidad;
                            detalle.producto = objDet.producto;
                            detalle.precio = objDet.precio;
                            detalle.subtotal = objDet.precio;
                            detalle.idinventario=objDet.idinventario;

                            detalle.total =  objDet.precio * detalle.cantidad - desc * detalle.cantidad;
                            detalle.Precio_desc = objDet.Precio_desc;
                            AgregarDetalleDevol(detalle, modal_Cant_Devol.cantidad);
                        }
                    }
                }
            }

        }
        public void mantenerDatos()
        {
            String saldo_total_string = String.Format("{0:N0}", saldo_total);
            lb_total.Content = "$ " + saldo_total_string;
        }

        private void btn_cancelar_devo_Click(object sender, RoutedEventArgs e)
        {
            var objDet = ((DetalleDTO)dgi_devolucion.SelectedItem);
            int sku = objDet.codigo;
            if (objDet.Cantidad == 1)
            {
                detDevol.Remove(objDet);
                var obj = detCambio.FirstOrDefault(x => x.codigo == sku);
                obj.CantidadDispo = obj.CantidadDispo + 1;

                int desc = obj.Precio_desc == null ? 0 : obj.Precio_desc.Value;
                obj.total = (obj.precio * obj.CantidadDispo)- desc*obj.CantidadDispo;
                

                saldo_total = saldo_total - ((objDet.precio * 1)-desc);
                mantenerDatos();
            }
            else
            {
                if (objDet.idmedida==1)
                {
                        detDevol.Remove(objDet);
                    var obj = detCambio.FirstOrDefault(x => x.codigo == sku);
                    int desc = obj.Precio_desc == null ? 0 : obj.Precio_desc.Value;
                    obj.CantidadDispo = obj.CantidadDispo + obj.cantidad;
                    obj.total = (obj.precio * obj.CantidadDispo) - desc;

                    saldo_total = saldo_total - ((objDet.precio * objDet.cantidad) - desc);
                    mantenerDatos();
                }
                else
                {
                    modal_cant_devol modal_Cant_Devol = new modal_cant_devol(objDet.Cantidad);
                    var modal = modal_Cant_Devol.ShowDialog();
                    if (modal == true)
                    {
                        if (objDet.Cantidad == modal_Cant_Devol.cantidad)
                        {
                            detDevol.Remove(objDet);
                        }
                        else
                        {
                            int de = objDet.Precio_desc == null ? 0 : objDet.Precio_desc.Value;
                            objDet.Cantidad = objDet.Cantidad - modal_Cant_Devol.cantidad;
                            objDet.total = (objDet.precio * objDet.Cantidad) - de * objDet.Cantidad;
                        }
                        var obj = detCambio.FirstOrDefault(x => x.codigo == sku);
                        int desc = obj.Precio_desc == null ? 0 : obj.Precio_desc.Value;
                        obj.CantidadDispo = obj.CantidadDispo + modal_Cant_Devol.cantidad;
                        obj.total = (obj.precio * obj.CantidadDispo) - desc * obj.CantidadDispo;

                        saldo_total = saldo_total - ((objDet.precio * modal_Cant_Devol.cantidad) - desc * modal_Cant_Devol.cantidad);
                        mantenerDatos();

                    }
                }
            }
        }
        private void mensaje(string msj)
        {
            ModalMensaje modal = new ModalMensaje(msj);
            modal.ShowDialog();
        }
        private int getNumber(TextBox text)
        {
            string valor = text.Text.Replace(".", "").ToString();
            int num = 0;
            if (int.TryParse(valor, out num))
            {
                return num;
            }
            return num;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txt_buscar_boleta.Text != null)
            {
                if (detDevol.Count > 0)
                {
                    if (radio_dev_efec.IsChecked == false && radio_cambio.IsChecked == false)
                    {
                        mensaje("Debe seleccionar CAMBIO O DEVOLUCIÓN");
                    }
                    else
                    {
                        if (radio_dev_efec.IsChecked == true)
                        {
                            HabilitarProceso hb = new HabilitarProceso("Habilitar Cambio",3);
                            bool? hab = hb.ShowDialog();
                            if (hab == true)
                            {
                                ModalMensaje msj = new ModalMensaje("CAMBIO AUTORIZADO", false, true);
                                msj.ShowDialog();


                                venta_devolucion venta_Devolucions = new venta_devolucion();
                                venta_Devolucions.idpedido = getNumber(txt_buscar_boleta);
                                venta_Devolucions.total_devolucion = saldo_total;
                                venta_Devolucions.motivo = txt_motivo.Text;
                                venta_Devolucions.idestadodevolucion = 1;
                                P.venta_devolucion.Add(venta_Devolucions);

                                int idventadevol = P.SaveChanges();
                                if (idventadevol > 0)
                                {
                                    foreach (var item in detDevol)
                                    {
                                        devo_producto devo_Producto = new devo_producto();
                                        devo_Producto.idventadevolucion = venta_Devolucions.idventadevolucion;
                                        devo_Producto.idproducto = item.codigo;
                                        devo_Producto.cantidad = item.Cantidad;
                                        devo_Producto.precio = item.precio;
                                        int desc = item.Precio_desc == null ? 0 : item.Precio_desc.Value;
                                        if (item.idmedida==1)
                                        {
                                            devo_Producto.total = item.precio * item.Cantidad - desc;
                                        }
                                        else
                                        {
                                            devo_Producto.total = item.precio * item.Cantidad - desc * item.Cantidad;
                                        }
                                         devo_Producto.idinventario = item.idinventario.Value;
                                        P.devo_producto.Add(devo_Producto);
                                    }
                                    int numatencion = 0;
                                    var obj = controllers.GetUltimaAtencion();
                                    atencion atencion = new atencion();
                                    if (obj != null)
                                    {
                                        numatencion = obj.numero_atencion + 1;
                                    }
                                    else
                                    {
                                        numatencion = 1;
                                    }
                                    atencion.idpedido = getNumber(txt_buscar_boleta);
                                    atencion.idventadevolucion = venta_Devolucions.idventadevolucion;
                                    atencion.idtipoatencion = 2;
                                    atencion.idestado_atencion = 1;

                                    atencion.numero_atencion = numatencion;
                                    atencion.fecha = DateTime.Now;
                                    atencion.vendedor = GlobalClass.user_nombre;
                                    P.atencion.Add(atencion);

                                    int idatencion = P.SaveChanges();


                                    if (idatencion > 0)
                                    {

                                        limpiar();
                                        //string ticketAtencion = GetAtencion(numatencion, GlobalClass.user_nombre);

                                        PrintDocument p = new PrintDocument();
                                        PaperSize paperSize = new PaperSize();
                                        paperSize.RawKind = (int)PaperKind.Custom;

                                        paperSize.Height = 2000;
                                        p.DefaultPageSettings.PaperSize = paperSize;
                                        p.PrintPage += delegate (object sender1, PrintPageEventArgs e1)
                                        {
                                            Font font = new Font("Arial Bold", 20);
                                            e1.Graphics.DrawString(numatencion.ToString(), font, new SolidBrush(System.Drawing.Color.Black), 40, 10);

                                            System.Drawing.Rectangle rectanguloQr =
                                            new System.Drawing.Rectangle(
                                                20, 50,
                                               250,
                                               70);


                                            var barcodeWriter = new BarcodeWriter
                                            {
                                                Format = BarcodeFormat.QR_CODE,
                                                Options = new ZXing.Common.EncodingOptions
                                                {
                                                    Height = 70,
                                                    Width = 250

                                                }
                                            };
                                            e1.Graphics.DrawImage(barcodeWriter.Write(numatencion.ToString()), rectanguloQr);
                                        };
                                        try
                                        {
                                            p.PrintController = new StandardPrintController();
                                            p.Print();

                                        }
                                        catch (Exception ex)
                                        {
                                            mensaje(ex.Message);
                                        }

                                    }
                                    else
                                    {
                                        mensaje("Ocurrio un error en el sistema");
                                    }
                                }
                                else
                                {
                                    mensaje("Ocurrio un error en el sistema");
                                }
                            }
                        }
                        if (radio_cambio.IsChecked == true)
                        {
                            HabilitarProceso hb = new HabilitarProceso("Habilitar Cambio",3);
                            bool? hab = hb.ShowDialog();
                            if (hab == true)
                            {
                                GlobalClass.saldo_devolucion = saldo_total;
                                ModalCamVen modalCamVen = new ModalCamVen();
                                var resul = modalCamVen.ShowDialog();
                                if (resul == true)
                                {

                                    ModalMensaje msj = new ModalMensaje("CAMBIO AUTORIZADO", false, true);
                                    msj.ShowDialog();

                                    venta_devolucion venta_Devolucions = new venta_devolucion();
                                    venta_Devolucions.idpedido = GlobalClass.idpedido;
                                    venta_Devolucions.total_devolucion = saldo_total;
                                    venta_Devolucions.motivo = txt_motivo.Text;
                                    venta_Devolucions.idestadodevolucion = 1;
                                    P.venta_devolucion.Add(venta_Devolucions);

                                    int idventadevol = P.SaveChanges();
                                    if (idventadevol > 0)
                                    {
                                        foreach (var item in detDevol)
                                        {
                                            devo_producto devo_Producto = new devo_producto();
                                            devo_Producto.idventadevolucion = venta_Devolucions.idventadevolucion;
                                            devo_Producto.idproducto = item.codigo;
                                            devo_Producto.cantidad = item.Cantidad;
                                            devo_Producto.precio = item.precio;
                                            devo_Producto.total = item.precio * item.Cantidad;

                                            devo_Producto.idinventario = item.idinventario.Value;
                                            P.devo_producto.Add(devo_Producto);
                                        }

                                        int numatencion = 0;
                                        var obj = controllers.GetUltimaAtencion();
                                        atencion atencion = new atencion();
                                        if (obj != null)
                                        {
                                            numatencion = obj.numero_atencion + 1;
                                        }
                                        else
                                        {
                                            numatencion = 1;
                                        }
                                        atencion.idventadevolucion = venta_Devolucions.idventadevolucion;
                                        atencion.idpedido = GlobalClass.idpedido;
                                        atencion.idtipoatencion = 3;
                                        atencion.idestado_atencion = 1;

                                        atencion.numero_atencion = numatencion;
                                        atencion.fecha = DateTime.Now;
                                        atencion.vendedor = GlobalClass.user_nombre;
                                        P.atencion.Add(atencion);

                                        int idatencion = P.SaveChanges();


                                        if (idatencion > 0)
                                        {
                                            foreach (var item in detDevol)
                                            {
                                                actualizarStockDevolucion(item.codigo, null, item.Cantidad);
                                            }


                                            limpiar();
                                            string ticketAtencion = GetAtencion(numatencion, GlobalClass.user_nombre);

                                            PrintDocument p = new PrintDocument();
                                            PaperSize paperSize = new PaperSize();
                                            paperSize.RawKind = (int)PaperKind.Custom;

                                            paperSize.Height = 2000;
                                            p.DefaultPageSettings.PaperSize = paperSize;
                                            p.PrintPage += delegate (object sender1, PrintPageEventArgs e1)
                                            {
                                                Font font = new Font("Arial Bold", 20);
                                                e1.Graphics.DrawString(numatencion.ToString(), font, new SolidBrush(System.Drawing.Color.Black), 40, 10);

                                                System.Drawing.Rectangle rectanguloQr =
                                                new System.Drawing.Rectangle(
                                                    20, 50,
                                                   250,
                                                   70);


                                                var barcodeWriter = new BarcodeWriter
                                                {
                                                    Format = BarcodeFormat.QR_CODE,
                                                    Options = new ZXing.Common.EncodingOptions
                                                    {
                                                        Height = 70,
                                                        Width = 250

                                                    }
                                                };
                                                e1.Graphics.DrawImage(barcodeWriter.Write(numatencion.ToString()), rectanguloQr);
                                            };
                                            try
                                            {
                                                p.PrintController = new StandardPrintController();
                                                p.Print();

                                            }
                                            catch (Exception ex)
                                            {
                                                mensaje(ex.Message);
                                            }

                                        }
                                        else
                                        {
                                            mensaje("Ocurrio un error en el sistema");
                                        }
                                    }
                                    else
                                    {
                                        mensaje("Ocurrio un error en el sistema");
                                    }


                                    limpiar();

                                }
                                else
                                {
                                    GlobalClass.saldo_devolucion = null;
                                }
                            }
                        }
                    }
                }
                else
                {
                    mensaje("Debe ingresar producto a la devolución");
                }
            }
            else
            {
                mensaje("Debe buscar la boleta");
            }
        }
        private void actualizarStockDevolucion(int idproducto, int? idinventario, int cantidad)
        {

            inventario inv = new inventario();
            operacion ope = new operacion();

            inv = P.inventario.FirstOrDefault(x => x.idproducto == idproducto);
            if (inv != null)
            {
                ope.idproducto = idproducto;
                ope.stock_ant = inv.stock_total;
                inv.stock_total = inv.stock_total + cantidad;
                ope.stock_act = inv.stock_total;

                ope.idtipo_operacion = 4;
                ope.idinventario = inv.idinventario;
                ope.fecha_hora = DateTime.Now;
                ope.cantidad = cantidad;

                P.operacion.Add(ope);

                P.SaveChanges();
            }
        }
        private void limpiar()
        {
            txt_buscar_boleta.Text = "";
            txt_motivo.Text = "";
            saldo_total = 0;
            mantenerDatos();
            detCambio.Clear();
            detDevol.Clear();
            productos.Clear();
            radio_cambio.IsChecked = null;
            radio_dev_efec.IsChecked = null;
            GlobalClass.saldo_devolucion = null;


        }
        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            limpiar();

        }
        public string GetAtencion(int numero, string vendedor)
        {



            StringBuilder textoComanda = new StringBuilder();
            string str = new string(' ', 5);
            string str2 = new string(' ', 20);
            string linea = new string('_', 25);

            textoComanda.AppendLine(str + "FERRETERIA" + str);
            textoComanda.AppendLine(str + "SAN ALFONSO" + str);
            textoComanda.AppendLine(str);
            textoComanda.AppendLine(linea);
            textoComanda.AppendLine(str);
            textoComanda.AppendLine("NUMERO DE ATENCIÓN");
            textoComanda.AppendLine(str);
            textoComanda.AppendLine(str2 + numero + str2);
            textoComanda.AppendLine("FECHA Y HORA");
            textoComanda.AppendLine(str + DateTime.Now.ToString() + str);
            textoComanda.AppendLine(str);
            textoComanda.AppendLine("VENDEDOR");
            textoComanda.AppendLine(str + vendedor + str);
            textoComanda.AppendLine(str);
            textoComanda.AppendLine(linea);

            return textoComanda.ToString();
        }
    }
}
