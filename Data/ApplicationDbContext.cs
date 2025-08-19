// Data/ApplicationDbContext.cs
using BioAlga.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Tablas
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Rol> Roles { get; set; } = null!;
        public DbSet<Empleado> Empleados { get; set; } = null!;
        public DbSet<Cliente> Clientes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === Usuarios ===
            modelBuilder.Entity<Usuario>(e =>
            {
                e.ToTable("usuarios");
                e.HasOne(u => u.Rol)
                    .WithMany()
                    .HasForeignKey(u => u.Id_Rol)
                    .HasConstraintName("fk_usuarios_roles");

                e.HasOne(u => u.Empleado)
                    .WithMany()
                    .HasForeignKey(u => u.Id_Empleado)
                    .HasConstraintName("fk_usuarios_empleados");
            });

            // === Clientes ===
            modelBuilder.Entity<Cliente>(e =>
            {
                e.ToTable("clientes");

                // Índices útiles
                e.HasIndex(c => c.Correo).IsUnique(false);   // pon true si quieres correo único
                e.HasIndex(c => c.Estado);
                e.HasIndex(c => c.Tipo_Cliente);

                // Defaults a nivel BD
                e.Property(c => c.Tipo_Cliente)
                    .HasDefaultValue("Normal");

                e.Property(c => c.Estado)
                    .HasDefaultValue("Activo");

                e.Property(c => c.Fecha_Registro)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Si usas MySQL y quieres ENUM reales:
                // e.Property(c => c.Tipo_Cliente).HasColumnType("ENUM('Normal','Mayorista','Premium')");
                // e.Property(c => c.Estado).HasColumnType("ENUM('Activo','Inactivo')");
            });
        }
    }
}
