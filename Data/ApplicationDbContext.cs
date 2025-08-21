// Data/ApplicationDbContext.cs
using BioAlga.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // ======= DbSets =======
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Rol> Roles { get; set; } = null!;
        public DbSet<Empleado> Empleados { get; set; } = null!;
        public DbSet<Cliente> Clientes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ======= Configuración global MySQL (Pomelo) =======
            // Asegura que todas las tablas usen utf8mb4 y collation insensible a mayúsculas.
            modelBuilder
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_unicode_ci");

            // ======= Usuarios =======
            modelBuilder.Entity<Usuario>(e =>
            {
                e.ToTable("usuarios");

                // Claves foráneas
                e.HasOne(u => u.Rol)
                    .WithMany()
                    .HasForeignKey(u => u.Id_Rol)
                    .HasConstraintName("fk_usuarios_roles");

                e.HasOne(u => u.Empleado)
                    .WithMany()
                    .HasForeignKey(u => u.Id_Empleado)
                    .HasConstraintName("fk_usuarios_empleados");
            });

            // ======= Clientes =======
            modelBuilder.Entity<Cliente>(e =>
            {
                e.ToTable("clientes");

                // Campos y restricciones (refuerza lo que ya definimos en el modelo)
                e.Property(c => c.Nombre)
                    .HasColumnName("nombre")
                    .HasMaxLength(100)
                    .IsRequired();

                e.Property(c => c.ApellidoPaterno)
                    .HasColumnName("apellido_paterno")
                    .HasMaxLength(100);

                e.Property(c => c.ApellidoMaterno)
                    .HasColumnName("apellido_materno")
                    .HasMaxLength(100);

                e.Property(c => c.Correo)
                    .HasColumnName("correo")
                    .HasMaxLength(100);

                e.Property(c => c.Telefono)
                    .HasColumnName("telefono")
                    .HasMaxLength(20);

                e.Property(c => c.Direccion)
                    .HasColumnName("direccion");

                e.Property(c => c.TipoCliente)
                    .HasColumnName("tipo_cliente")
                    .HasMaxLength(20);

                e.Property(c => c.Estado)
                    .HasColumnName("estado")
                    .HasMaxLength(20);

                e.Property(c => c.FechaRegistro)
                    .HasColumnName("fecha_registro");

                // Índices
                e.HasIndex(c => new { c.Nombre, c.ApellidoPaterno, c.ApellidoMaterno })
                    .HasDatabaseName("idx_clientes_nombre");

                e.HasIndex(c => c.Correo)
                    .IsUnique()
                    .HasDatabaseName("UX_clientes_correo"); // coincide con UNIQUE de la BD
            });

          

            // (Opcional) Roles: nombre único ya está por DB, pero lo reflejamos en EF
            modelBuilder.Entity<Rol>(e =>
            {
                e.ToTable("roles");
                e.HasIndex(r => r.Nombre)
                    .IsUnique()
                    .HasDatabaseName("UX_roles_nombre");
            });
        }
    }
}
