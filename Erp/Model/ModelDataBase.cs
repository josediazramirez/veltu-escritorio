namespace Model
{
    using System.Data.Entity;
    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public partial class ModelDataBase : DbContext
    {
        public ModelDataBase()
            : base("name=ConexionSistema")
        {
        }

        public virtual DbSet<Caja> Caja { get; set; }
        public virtual DbSet<Categoria> Categoria { get; set; }
        public virtual DbSet<Cliente> Cliente { get; set; }
        public virtual DbSet<Computador> Computador { get; set; }
        public virtual DbSet<Detalle_pedido> Detalle_pedido { get; set; }
        public virtual DbSet<MedioPago> MedioPago { get; set; }
        public virtual DbSet<Pedido> Pedido { get; set; }
        public virtual DbSet<Producto> Producto { get; set; }
        public virtual DbSet<agregado> agregado { get; set; }
        public virtual DbSet<pro_agre> pro_agre { get; set; }
        public virtual DbSet<medida> medida { get; set; }
        public virtual DbSet<centro> centro { get; set; }
        public virtual DbSet<inventario> inventario { get; set; }
        public virtual DbSet<operacion> operacion { get; set; }
        public virtual DbSet<estado> estado { get; set; }
        public virtual DbSet<rol> rol { get; set; }
        public virtual DbSet<usuario> usuario { get; set; }
        public virtual DbSet<marca> marca { get; set; }
        public virtual DbSet<color> color { get; set; }
        public virtual DbSet<mov_caja> mov_caja { get; set; }
        public virtual DbSet<pagomov> pagomov { get; set; }
        public virtual DbSet<venta_devolucion> venta_devolucion { get; set; }
        public virtual DbSet<atencion> atencion { get; set; }
        public virtual DbSet<devo_producto> devo_producto { get; set; }
        public virtual DbSet<transferencia_bodega> transferencia_bodega { get; set; }
        public virtual DbSet<usuario_proceso> usuario_proceso { get; set; }
        public virtual DbSet<autorizacion_proceso> autorizacion_proceso { get; set; }
        public virtual DbSet<tipo_operacion> tipo_operacion { get; set; }
        public virtual DbSet<factura> facturas { get; set; }
    }
}
