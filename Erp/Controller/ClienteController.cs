using System.Collections.Generic;
using System.Linq;
using DTO;
using System.Data;
using Model;
using MySql.Data.MySqlClient;

namespace Controller
{
    public class ClienteController
    {
        ModelDataBase db = new ModelDataBase();
        public Cliente guardarCliente(clienteDTO cliente)
        {
            try
            {
                
          
                Cliente cl = new Cliente();
                cl.id_telefono = cliente.id_telefono;
                cl.nombre = cliente.nombre;
                cl.direccion = cliente.direccion;
                cl.num_direccion = cliente.num_direccion;
                cl.telefono_opc = cliente.telefono_opc;

                db.Cliente.Add(cl);
                db.SaveChanges();
                return cl;

            }
            catch (MySqlException ex)
            {

                throw ex;
            }
        }
        public int RemoveCliente(int idcliente)
        {
            Cliente user = db.Cliente.FirstOrDefault(x => x.idcliente == idcliente);
            db.Cliente.Remove(user);
            return db.SaveChanges();
        }
        public int EditCliente(clienteDTO cli)
        {
            try
            {
                Cliente cliente = db.Cliente.FirstOrDefault(x => x.idcliente == cli.idcliente);

                cliente.nombre = cli.nombre;
                cliente.direccion = cli.direccion;
                cliente.num_direccion = cli.num_direccion;
                cliente.telefono_opc = cli.telefono_opc;

                return db.SaveChanges();
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            
        }
        public List<clienteDTO> GetClientes()
        {
            List<clienteDTO> lista = new List<clienteDTO>();
            try
            {
                

            lista = (from clien in db.Cliente
                     select new clienteDTO
                     {
                         idcliente = clien.idcliente,
                         id_telefono = clien.id_telefono,
                         nombre = clien.nombre,
                         direccion = clien.direccion,
                         num_direccion = clien.num_direccion,
                         telefono_opc = clien.telefono_opc
                     }).ToList();
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            return lista;
        }
        public List<clienteDTO> getCliente(int idcliente)
        {
            List<clienteDTO> lista = new List<clienteDTO>();
            try
            {


                lista = (from clien in db.Cliente
                         where clien.idcliente== idcliente
                         select new clienteDTO
                         {
                             idcliente = clien.idcliente,
                             id_telefono = clien.id_telefono,
                             nombre = clien.nombre,
                             direccion = clien.direccion,
                             num_direccion = clien.num_direccion,
                             telefono_opc = clien.telefono_opc

                         }).ToList();
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            return lista;
        }
    }
}
