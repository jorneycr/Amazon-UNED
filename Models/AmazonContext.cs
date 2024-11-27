using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

public class AmazonContext : IdentityDbContext<Usuario>
{
    public AmazonContext(DbContextOptions<AmazonContext> options) : base(options) { }

    // Define DbSet para cada modelo de la aplicación
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<DetallePedido> DetallePedidos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar relaciones y restricciones

        // Relación Usuario -> Pedido (1 a muchos)
        modelBuilder.Entity<Usuario>()
            .HasMany(u => u.HistorialPedidos)
            .WithOne(p => p.Usuario)
            .HasForeignKey(p => p.UsuarioId);

        modelBuilder.Entity<Pedido>()
        .HasMany(p => p.Detalles)
        .WithOne(d => d.Pedido)
        .HasForeignKey(d => d.PedidoId)
        .OnDelete(DeleteBehavior.Cascade);

        // Relación Pedido -> DetallePedido (1 a muchos)
        modelBuilder.Entity<DetallePedido>()
            .HasOne(dp => dp.Pedido)
            .WithMany(p => p.Detalles)
            .HasForeignKey(dp => dp.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación Producto -> DetallePedido (1 a muchos)
        modelBuilder.Entity<DetallePedido>()
            .HasOne(dp => dp.Producto)
            .WithMany()
            .HasForeignKey(dp => dp.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configurar propiedades de tipo decimal
        modelBuilder.Entity<DetallePedido>()
            .Property(dp => dp.Precio)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Pedido>()
            .Property(p => p.PrecioTotal)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Producto>()
            .Property(p => p.Precio)
            .HasPrecision(18, 2);

        // Configurar unicidad y restricciones adicionales
        modelBuilder.Entity<Producto>()
            .Property(p => p.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<Usuario>()
            .Property(u => u.Nombre)
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<Usuario>()
            .Property(u => u.Apellido)
            .IsRequired()
            .HasMaxLength(50);
    }

    // Método para inicializar roles
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Crear rol "Usuario" si no existe
        if (!await roleManager.RoleExistsAsync("Usuario"))
        {
            await roleManager.CreateAsync(new IdentityRole("Usuario"));
        }

        // Crear rol "Admin" si no existe
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }
    }

    public static async Task SeedProductosAsync(AmazonContext context)
    {
        if (!context.Productos.Any()) // Verifica si ya hay productos en la base de datos
        {
            var productos = new List<Producto>
        {
            new Producto { Nombre = "Laptop HP", Descripcion = "Laptop de alto rendimiento con 16GB RAM y SSD de 512GB", Precio = 1200.00m, Categoria = "Tecnología", Stock = 15 },
            new Producto { Nombre = "Mouse Logitech", Descripcion = "Mouse inalámbrico ergonómico", Precio = 25.00m, Categoria = "Accesorios", Stock = 50 },
            new Producto { Nombre = "Audífonos Sony", Descripcion = "Audífonos con cancelación de ruido", Precio = 150.00m, Categoria = "Electrónica", Stock = 30 },
            new Producto { Nombre = "Monitor Samsung", Descripcion = "Monitor 4K UHD de 27 pulgadas", Precio = 350.00m, Categoria = "Tecnología", Stock = 10 },
            new Producto { Nombre = "Teclado Mecánico", Descripcion = "Teclado mecánico retroiluminado", Precio = 80.00m, Categoria = "Accesorios", Stock = 20 },
            new Producto { Nombre = "Silla Gamer", Descripcion = "Silla ergonómica para gamers con soporte lumbar", Precio = 200.00m, Categoria = "Muebles", Stock = 5 }
        };

            await context.Productos.AddRangeAsync(productos);
            await context.SaveChangesAsync();
        }
    }

}
