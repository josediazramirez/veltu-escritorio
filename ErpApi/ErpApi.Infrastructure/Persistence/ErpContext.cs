using ErpApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpApi.Infrastructure.Persistence;

public class ErpContext : DbContext
{
    public ErpContext(DbContextOptions<ErpContext> options) : base(options)
    {
    }

    public DbSet<Producto> Productos { get; set; } = null!;
    public DbSet<Inventario> Inventarios { get; set; } = null!;
    public DbSet<Categoria> Categorias { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("Producto");
            entity.HasKey(e => e.Codigo);
        });

        modelBuilder.Entity<Inventario>(entity =>
        {
            entity.ToTable("inventario");
            entity.HasKey(e => e.IdInventario);
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("Categoria");
            entity.HasKey(e => e.Codigo);
        });
    }
}
