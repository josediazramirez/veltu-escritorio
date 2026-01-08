using DTO;
using Model;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Cmp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Net.WebRequestMethods;

namespace Controller
{
    public class ProductoController
    {
        ModelDataBase db = new ModelDataBase();
        public List<UnidadMedidaDTO> getUnidadMedida()
        {
            List<UnidadMedidaDTO> lista = new List<UnidadMedidaDTO>();
            try
            {
                lista = db.medida.Select(x => new UnidadMedidaDTO
                {
                    codigo = x.idmedida,
                    nombre = x.nombre,
                    sigla = x.sigla
                }).ToList();
            }
            catch (EntityException)
            {
                throw new EntityException();
            }
            return lista;
        }
        public List<productoDTO> getFiltroProducto(string texto)
        {
            List<productoDTO> lista_product = new List<productoDTO>();
            try
            {
                lista_product = db.Database
                         .SqlQuery<productoDTO>("sp_producto_filtro_text(@txt)", new MySqlParameter("@txt", texto))
                         .ToList();
            }
            catch (Exception ex)
            {
                var msj = ex.Message;
            }

            return lista_product;
        }
        public List<productoDTO> getFiltroProductoFull(string texto, int idcentro)
        {
            List<productoDTO> lista_product = new List<productoDTO>();
            try
            {
                lista_product = db.Database
                         .SqlQuery<productoDTO>("sp_producto_filtro_full(@txt,@idcentro)", new MySqlParameter("@txt", texto), new MySqlParameter("@idcentro", idcentro))
                         .ToList();
            }
            catch (Exception ex)
            {
                var msj = ex.Message;
            }
            return lista_product;
        }
        public List<productoDTO> getProductoXcodigo(long codigo)
        {
            List<productoDTO> lista_product = new List<productoDTO>();
            try
            {
                lista_product = db.Database
                         .SqlQuery<productoDTO>("sp_producto_x_codigo(@codigo)", new MySqlParameter("@codigo", codigo))
                         .ToList();
            }
            catch (Exception ex)
            {
                var msj = ex.Message;
            }
            return lista_product;
        }
        public List<productoDTO> getFlete()
        {
            List<productoDTO> lista_product = new List<productoDTO>();
            try
            {
                lista_product = db.Database
                         .SqlQuery<productoDTO>("sp_flete_valores")
                         .ToList();
            }
            catch (Exception ex)
            {
                var msj = ex.Message;
            }
            return lista_product;
        }
        public List<productoDTO> getGrillaProductos(int? codigo)
        {
            List<productoDTO> lista = new List<productoDTO>();
            string baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            try
            {
                if (codigo == null)
                {
                    lista =
                        (from pro in db.Producto
                         join cat in db.Categoria
                         on pro.categoria_codigo equals cat.codigo
                         join co in db.color
                        on pro.idcolor equals co.idcolor
                        into colors
                         from col in colors.DefaultIfEmpty()
                         join mar in db.marca
                        on pro.idmarca equals mar.idmarca
                        into mars
                         from marc in mars.DefaultIfEmpty()
                         orderby pro.codigo descending

                         select new productoDTO
                         {
                             codigo = pro.codigo,
                             nombre = pro.nombre,
                             precio = pro.precio.Value,
                             categoria = cat.codigo,
                             name_categoria = cat.nombre.ToUpper(),
                             marca = marc.nombre,
                             color = col.nombre,
                             idmarca = marc.idmarca,
                             idcolor = col.idcolor,
                             ean = pro.ean_codigo,
                             precio_costo = pro.precio_costo,
                             estado = pro.estado,
                             descripcion = pro.descripcion,
                             lote = pro.lote,
                             ImagenRuta = pro.imagen1
                         }).ToList();
                }
                else
                {
                    (from pro in db.Producto
                     join cat in db.Categoria
                        on pro.categoria_codigo equals cat.codigo
                     join co in db.color
                    on pro.idcolor equals co.idcolor
                    into colors
                     from col in colors.DefaultIfEmpty()
                     join mar in db.marca
                    on pro.idmarca equals mar.idmarca
                    into mars
                     from marc in mars.DefaultIfEmpty()
                     where pro.categoria_codigo == codigo
                     orderby pro.codigo descending
                     select new productoDTO
                     {
                         codigo = pro.codigo,
                         nombre = pro.nombre,
                         precio = pro.precio.Value,
                         categoria = cat.codigo,
                         name_categoria = cat.nombre.ToUpper(),
                         marca = marc.nombre,
                         color = col.nombre,
                         idmarca = marc.idmarca,
                         idcolor = col.idcolor,
                         ean = pro.ean_codigo,
                         precio_costo = pro.precio_costo,
                         estado = pro.estado,
                         descripcion = pro.descripcion,
                         lote = pro.lote,
                         ImagenRuta = pro.imagen1
                     }).ToList();
                }



            }
            catch (EntityException ex)
            {
                throw new EntityException();
            }
            foreach (var item in lista)
            {
                item.ImagenRuta = $"{baseUrl}/{item.ImagenRuta}";
                if (item.precio_costo != null)
                {
                    item.utilidad = item.precio - item.precio_costo.Value;
                }
            }

            return lista;
        }
        public List<productoInvDTO> getProductoInv(int? categoria)
        {
            List<productoInvDTO> lista = new List<productoInvDTO>();
            try
            {
                if (categoria != 0)
                {
                    lista = db.Database
                    .SqlQuery<productoInvDTO>("sp_inventario_productos_get(@categoria)", new MySqlParameter("@categoria", categoria.Value))
                    .ToList();
                }

            }
            catch (EntityException)
            {
                throw new EntityException();
            }
            return lista;
        }
        public List<CategoriaDTO> getCentro()
        {
            List<CategoriaDTO> lista = new List<CategoriaDTO>();
            try
            {
                lista = (from centro in db.centro
                         where centro.estado == 1
                         select new CategoriaDTO
                         {
                             codigo = centro.idcentro,
                             nombre = centro.nombre.ToUpper()
                         }).ToList();
            }
            catch (EntityException)
            {
                throw new EntityException();
            }
            return lista;
        }
        public List<CategoriaDTO> getCategorias()
        {
            List<CategoriaDTO> lista = new List<CategoriaDTO>();
            try
            {
                lista = (from cate in db.Categoria
                         where cate.estado == 1
                         select new CategoriaDTO
                         {
                             codigo = cate.codigo,
                             nombre = cate.nombre.ToUpper()
                         }).ToList();
            }
            catch (EntityException)
            {
                throw new EntityException();
            }
            return lista;
        }
        public List<InventarioDTO> geInventario()
        {
            List<InventarioDTO> lista = new List<InventarioDTO>();
            try
            {
                lista = db.Database
                          .SqlQuery<InventarioDTO>("sp_inventario_get").ToList();
            }
            catch (EntityException)
            {
                throw new EntityException();
            }
            return lista;
        }
        public List<MovimientoDTO> getMovimiento()
        {
            List<MovimientoDTO> lista = new List<MovimientoDTO>();
            try
            {
                lista = db.Database
                 .SqlQuery<MovimientoDTO>("sp_inventario_mov")
                 .ToList();
            }
            catch (EntityException)
            {
                throw new EntityException();
            }
            return lista;
        }
        public List<MovimientoDTO> getMovimientoxfecha(DateTime fecha_ini, DateTime fecha_fin, string producto, string tipo_ope)
        {

            List<MovimientoDTO> lista = new List<MovimientoDTO>();
            try
            {
                lista = db.Database
                 .SqlQuery<MovimientoDTO>("sp_inventario_mov_x_fecha(@p_fecha_ini,@p_fecha_fin,@p_producto,@p_tipo_ope)",
                 new MySqlParameter("@p_fecha_ini", fecha_ini),
                 new MySqlParameter("@p_fecha_fin", fecha_fin),
                 new MySqlParameter("@p_producto", producto),
                 new MySqlParameter("@p_tipo_ope", tipo_ope))
                 .ToList();
            }
            catch (EntityException)
            {
                throw new EntityException();
            }
            return lista;
        }
        public async Task<int> guardarProducto(productoDTO producto, byte[] image)
        {
            int insert = 0;
            try
            {


                Producto pro = new Producto();
                pro.nombre = producto.nombre.ToUpper();
                pro.precio = producto.precio;
                pro.categoria_codigo = producto.categoria;
                pro.idcolor = producto.idcolor;
                pro.idmarca = producto.idmarca;
                pro.ean_codigo = producto.ean;
                pro.estado = 1;
                pro.precio_costo = producto.precio_costo;
                pro.descripcion = producto.descripcion;
                pro.lote = producto.lote;

                insert = db.Producto.FirstOrDefault(x => x.nombre == producto.nombre
                && x.idmarca == producto.idmarca && x.idcolor == producto.idcolor) != null ? 1 : 0;

                if (insert == 1)
                {
                    return 1;
                }
                else
                {

                    using (ModelDataBase bd = new ModelDataBase())
                    {

                        bd.Producto.Add(pro);
                        bd.SaveChanges();
                    }

                    inventario inven = new inventario();
                    inven.idproducto = pro.codigo;
                    inven.idcentro = 1;
                    inven.idmedida = 3;
                    inven.stock_total = 0;
                    inven.stock_minimo = 0;
                    db.inventario.Add(inven);
                    db.SaveChanges();

                    await guardarImagenAsync(pro.codigo, image);



                    return 0;
                }

            }
            catch (Exception ex)
            {

                throw new Exception();
            }
        }
        public async Task guardarImagenAsync(int idproducto, byte[] imagen)
        {
            if (imagen != null)
            {


                using (var client = new HttpClient())
                using (var form = new MultipartFormDataContent())
                {
                    client.DefaultRequestHeaders.Add("ApiKey", "lkanfn12213in12312aXSAsd21312");
                    // idproducto como parte del form
                    form.Add(new StringContent(idproducto.ToString()), "idproducto");

                    // imagen como archivo (usando el byte[])
                    var imageContent = new ByteArrayContent(imagen);
                    imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                    form.Add(imageContent, "img1", "imagen.jpg");

                    string baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];

                    var response = await client.PostAsync($"{baseUrl}/ProductosAll/GuardarImagenProducto", form);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        //MessageBox.Show("Imagen enviada correctamente: " + result);
                    }
                    else
                    {
                        MessageBox.Show("Error al enviar imagen: " + response.StatusCode);
                    }
                }
            }
        }
        public int RemoveProducto(int codigo)
        {

            try
            {
                int cant = db.Detalle_pedido.Where(x => x.Producto.codigo == codigo).Count();
                if (cant > 0)
                {
                    return 1;
                }
                else
                {
                    inventario inv = db.inventario.FirstOrDefault(x => x.idproducto == codigo);
                    if (inv != null)
                    {
                        db.inventario.Remove(inv);
                    }

                    Producto pro = db.Producto.FirstOrDefault(x => x.codigo == codigo);
                    db.Producto.Remove(pro);
                    db.SaveChanges();
                    return 2;
                }

            }
            catch (EntityException)
            {
                throw new EntityException();
            }

        }
        public int RemoveProductoInv(int codigo)
        {
            int result = 0;
            try
            {

                inventario inv = db.inventario.FirstOrDefault(x => x.idinventario == codigo);
                if (inv != null)
                {
                    db.inventario.Remove(inv);
                }

                result = db.SaveChanges();
                if (result > 0)
                {
                    return 1;
                }
                else
                {

                    return 2;
                }

            }
            catch (EntityException)
            {
                throw new EntityException();
            }

        }
        public async Task<int> EditProducto(productoDTO pro, byte[] image)
        {
            Producto producto = db.Producto.FirstOrDefault(x => x.codigo == pro.codigo);

            producto.nombre = pro.nombre;
            producto.precio = pro.precio;
            producto.categoria_codigo = pro.categoria;
            producto.ean_codigo = pro.ean;
            producto.idcolor = pro.idcolor;
            producto.idmarca = pro.idmarca;
            producto.precio_costo = pro.precio_costo;
            producto.descripcion = pro.descripcion;
            producto.lote = pro.lote;

            var inv = db.inventario.FirstOrDefault(x => x.idproducto == producto.codigo);
            if (inv == null)
            {
                inventario inven = new inventario();
                inven.idproducto = producto.codigo;
                inven.idcentro = 1;
                inven.idmedida = 3;
                inven.stock_total = 0;
                inven.stock_minimo = 0;
                db.inventario.Add(inven);
            }
            await guardarImagenAsync(pro.codigo, image);
            return db.SaveChanges();
        }
        public int EditarInventario(InventarioDTO inv)
        {
            inventario invent = db.inventario.FirstOrDefault(x => x.idinventario == inv.cod_inv);

            invent.idproducto = inv.cod_pro;
            invent.idmedida = inv.iduni_medida;
            if (invent.idmedida == 1)
            {
                inv.stock_minimo = inv.stock_minimo.Replace(".", ",");
                inv.stock_total = inv.stock_total.Replace(".", ",");
                invent.stock_minimo = (int)(decimal.Parse(inv.stock_minimo) * 1000);
                invent.stock_total = (int)(decimal.Parse(inv.stock_total) * 1000);
            }
            else
            {
                invent.stock_minimo = int.Parse(inv.stock_minimo);
                invent.stock_total = int.Parse(inv.stock_total);
            }
            invent.idcentro = inv.idubicacion;




            return db.SaveChanges();
        }

        public int GuardarInventario(InventarioDTO inv)
        {
            inventario invent = new inventario();

            invent.idproducto = inv.cod_pro;
            invent.idmedida = inv.iduni_medida;
            if (invent.idmedida == 1)
            {
                inv.stock_minimo = inv.stock_minimo.Replace(".", ",");
                inv.stock_total = inv.stock_total.Replace(".", ",");
                invent.stock_minimo = (int)(decimal.Parse(inv.stock_minimo) * 1000);
                invent.stock_total = (int)(decimal.Parse(inv.stock_total) * 1000);
            }
            else
            {
                invent.stock_minimo = int.Parse(inv.stock_minimo);
                invent.stock_total = int.Parse(inv.stock_total);
            }
            invent.idcentro = inv.idubicacion;



            db.inventario.Add(invent);
            return db.SaveChanges();
        }
        public List<centro> GetCentros()
        {
            List<centro> list = new List<centro>();
            list.Add(new centro { idcentro = 0, nombre = "Seleccione" });
            list.AddRange(db.centro.ToList());
            return list;
        }

    }
}
