using Microsoft.EntityFrameworkCore;

namespace Facturacion.Data;

public class FacturacionDbContext : DbContext
{
    public FacturacionDbContext(DbContextOptions<FacturacionDbContext> options)
        : base(options)
    {
    }

    public DbSet<Factura> Facturas { get; set; }
    public DbSet<Articulo> Articulos { get; set; }
}

public class Factura
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public List<Articulo> Articulos { get; set; } = new();
    public decimal Total => Articulos?.Sum(a => a.Precio) ?? 0;
}

public class Articulo
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int FacturaId { get; set; }
}