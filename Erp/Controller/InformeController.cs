using DTO;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity.Core;
using Model;
using System;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Controller
{
    public class InformeController
    {
        ModelDataBase db = new ModelDataBase();
        public async Task<List<CajDTO>> getCajaxdia(DateTime fecha)
        {

            List<CajDTO> lista = new List<CajDTO>();
            try
            {
                lista = await db.Database
            .SqlQuery<CajDTO>("sp_caja_x_fecha(@fecha)", new MySqlParameter("@fecha", fecha))
            .ToListAsync();
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            return lista;
        }
        public async Task<List<movcajdto>> getCajaDet(int idcaja)
        {

            List<movcajdto> lista = new List<movcajdto>();
            try
            {
                lista = await db.Database
            .SqlQuery<movcajdto>("sp_caja_x_id(@idcaja)", new MySqlParameter("@idcaja", idcaja))
            .ToListAsync();
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            return lista;
        }
        public async Task<List<pedidoDTO>> getPedidos(DateTime fecha)
        {

            List<pedidoDTO> lista = new List<pedidoDTO>();
            try
            {
                lista = await db.Database
            .SqlQuery<pedidoDTO>("sp_venta_x_dia(@fecha)", new MySqlParameter("@fecha", fecha))
            .ToListAsync();
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            return lista;
        }
        public List<ComandaDTO> getComanda(int codigo)
        {
            List<ComandaDTO> lista = new List<ComandaDTO>();
            try
            {
                lista = db.Database
            .SqlQuery<ComandaDTO>("sp_venta_x_id(@idpedido)", new MySqlParameter("@idpedido", codigo))
            .ToList();

            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            return lista;
        }
        public FacturaDTO CrearFactura(FacturaDTO factura)
        {
            FacturaDTO nuevaFactura = null;
            try
            {
                nuevaFactura = db.Database
                    .SqlQuery<FacturaDTO>(
                        "CALL sp_insert_factura(@rut, @correo, @numero, @idpedido, @fecha, @estado)",
                        new MySqlParameter("@rut", factura.rut),
                        new MySqlParameter("@correo", factura.correo),
                        new MySqlParameter("@numero", factura.numero),
                        new MySqlParameter("@idpedido", factura.idpedido),
                        new MySqlParameter("@fecha", factura.fecha),
                        new MySqlParameter("@estado", factura.estado)
                    )
                    .FirstOrDefault();
            }
            catch (MySqlException ex)
            {
                throw; // puedes loguear antes de relanzar
            }
            return nuevaFactura;
        }
        public atencion GetUltimaAtencion()
        {

            var fecha = DateTime.Now.Date;
            atencion obj = (from ate in db.atencion
                          where DbFunctions.TruncateTime(ate.fecha) == fecha
                          orderby ate.idatencion descending
                          select ate).Take(1).FirstOrDefault();
            return obj;
        }
    }
}
