using DTO;
using Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Data.Entity;

namespace Controller
{
    public class CajaArqueoContro
    {
        readonly ModelDataBase db = new ModelDataBase();
        public async Task<CajaDTO> GetUltCaja(int idusuario)
        {
            DateTime now = DateTime.Now.Date;

            Task<CajaDTO> objCa = (from ca in db.Caja
                                   join user in db.usuario
                                   on ca.idusuario equals user.idusuario
                                   where ca.idusuario == idusuario
                                   orderby ca.codigo descending
                                   select new CajaDTO
                                   {
                                       codigo = ca.codigo,
                                       idusuario = ca.idusuario,
                                       estado = ca.estado,
                                       user_nombre = user.nombre,
                                       user_apellido = user.ape_paterno,
                                       fecha = ca.fecha,
                                       hora_inicio = ca.hora_inicio,
                                       hora_termino = ca.hora_termino,
                                       saldo_inicial = ca.efectivo_inicio,
                                       saldo_actual = ca.efectivo_esperado,
                                       saldo_diferencia = ca.efectivo_diferencia,
                                       saldo_cierre = ca.efectivo_hay

                                   }).FirstOrDefaultAsync();

            return await objCa;
        }
        public Task<List<arqueoDTO>> GetIngresos(int idcaja)
        {
            var result = db.Database.
                    SqlQuery<arqueoDTO>("sp_caja_x_id_ingresos(@idcaja)", new MySqlParameter("@idcaja", idcaja)).ToListAsync();

            return result;
        }
        public Task<List<arqueoDTO>> GetEgresos(int idcaja)
        {
            var result = db.Database.
                    SqlQuery<arqueoDTO>("sp_caja_x_id_egresos(@idcaja)", new MySqlParameter("@idcaja", idcaja)).ToListAsync();

            return result;
        }
        public Task<List<MovCajaDTO>> GetMovIngEgre(int idcaja)
        {
            var result = db.Database.
                    SqlQuery<MovCajaDTO>("sp_caja_x_id_ing_egre(@idcaja)", new MySqlParameter("@idcaja", idcaja)).ToListAsync();

            return result;
        }
        public async Task<int> SaveMov(int idcaja, int idtipo, string descripcion, int total,int tipopago)
        {
            int result = 0;
            mov_caja mov_ = new mov_caja();
            mov_.idcaja = idcaja;
            mov_.idtipomov = idtipo;
            mov_.observacion = descripcion;
            mov_.mov_fecha = DateTime.Now;
            if (idtipo == 4)
            {
                mov_.total_ent = total;
            }
            else if (idtipo == 5)
            {
                mov_.total_sal = total;
            }

            db.mov_caja.Add(mov_);
            var re = await db.SaveChangesAsync();

            if (re > 0)
            {
                pagomov pagomov = new pagomov();
                pagomov.descuento = 0;
                pagomov.idmovcaja = mov_.idmovcaja;
                if (idtipo == 4)
                {
                    pagomov.total = mov_.total_ent;
                }
                else if (idtipo == 5)
                {
                    pagomov.total = mov_.total_sal;
                }

                pagomov.vuelto = 0;
                pagomov.mp_id = tipopago;
                db.pagomov.Add(pagomov);
                result = await db.SaveChangesAsync();
            }
            return result;
        }
        public async Task<Caja> CreateUpdateNewCaja(int idcaja, int idusuario, string estado, int saldo_inicial, int saldo_esperado, int saldo_hay, int saldo_diferencia)
        {
            Caja caja = new Caja();
            if (estado == "Inicio")
            {
                caja.fecha = DateTime.Now.Date;
                caja.hora_inicio = DateTime.Now;
                caja.efectivo_inicio = saldo_inicial;
                caja.estado = estado;
                caja.idusuario = idusuario;
                db.Caja.Add(caja);
            }
            else
            {
                caja = await db.Caja.FirstOrDefaultAsync(x => x.codigo == idcaja);
                caja.hora_termino = DateTime.Now;
                caja.estado = estado;
                caja.efectivo_hay = saldo_hay;
                caja.efectivo_esperado = saldo_esperado;
                caja.efectivo_diferencia = saldo_diferencia;

            }

            await db.SaveChangesAsync();

            return caja;
        }
        public async Task<int> DeleteMov(int idmov_caja)
        {
            mov_caja mov = await db.mov_caja.FirstOrDefaultAsync(x => x.idmovcaja == idmov_caja);
            db.mov_caja.Remove(mov);
            return await db.SaveChangesAsync();
        }
    }
}
