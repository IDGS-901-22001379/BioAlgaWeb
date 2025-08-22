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
            modelBuilder
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_unicode_ci");

            // ============================================
            // USUARIOS
            // ============================================
            modelBuilder.Entity<Usuario>(e =>
            {
                e.ToTable("usuarios");

                // FK: usuarios.id_rol -> roles.id_rol (N:1)
                e.HasOne(u => u.Rol)
                    .WithMany()
                    .HasForeignKey(u => u.Id_Rol)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_usuarios_roles");

                // FK 1–1: usuarios.id_empleado -> empleados.id_empleado
                // (navegación inversa Empleado.Usuario)
                e.HasOne(u => u.Empleado)
                    .WithOne(emp => emp.Usuario)
                    .HasForeignKey<Usuario>(u => u.Id_Empleado)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_usuarios_empleados");
            });

            // ============================================
            // EMPLEADOS
            // ============================================
            modelBuilder.Entity<Empleado>(e =>
            {
                e.ToTable("empleados");

                // Columnas y tamaños (refuerza el modelo)
                e.Property(x => x.Nombre)
                    .HasColumnName("nombre")
                    .HasMaxLength(100)
                    .IsRequired();

                e.Property(x => x.Apellido_Paterno)
                    .HasColumnName("apellido_paterno")
                    .HasMaxLength(100);

                e.Property(x => x.Apellido_Materno)
                    .HasColumnName("apellido_materno")
                    .HasMaxLength(100);

                e.Property(x => x.Curp)
                    .HasColumnName("curp")
                    .HasMaxLength(18);

                e.Property(x => x.Rfc)
                    .HasColumnName("rfc")
                    .HasMaxLength(13);

                e.Property(x => x.Correo)
                    .HasColumnName("correo")
                    .HasMaxLength(120);

                e.Property(x => x.Telefono)
                    .HasColumnName("telefono")
                    .HasMaxLength(20);

                e.Property(x => x.Puesto)
                    .HasColumnName("puesto")
                    .HasMaxLength(80);

                e.Property(x => x.Salario)
                    .HasColumnName("salario")
                    .HasColumnType("decimal(10,2)");

                e.Property(x => x.Fecha_Ingreso)
                    .HasColumnName("fecha_ingreso")
                    .HasColumnType("date");

                e.Property(x => x.Fecha_Baja)
                    .HasColumnName("fecha_baja")
                    .HasColumnType("date");

                e.Property(x => x.Estatus)
                    .HasColumnName("estatus")
                    .HasMaxLength(10)
                    .HasDefaultValue("Activo");

                e.Property(x => x.Created_At)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                e.Property(x => x.Updated_At)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                // Índices útiles para búsqueda por nombre y correo
                e.HasIndex(x => new { x.Nombre, x.Apellido_Paterno, x.Apellido_Materno })
                    .HasDatabaseName("idx_empleados_nombre");

                // Si en tu BD el correo es UNIQUE, cambia IsUnique(true)
                e.HasIndex(x => x.Correo)
                    .IsUnique(false)
                    .HasDatabaseName("idx_empleados_correo");
            });

            // ============================================
            // CLIENTES
            // ============================================
            modelBuilder.Entity<Cliente>(e =>
            {
                e.ToTable("clientes");

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

                e.HasIndex(c => new { c.Nombre, c.ApellidoPaterno, c.ApellidoMaterno })
                    .HasDatabaseName("idx_clientes_nombre");

                e.HasIndex(c => c.Correo)
                    .IsUnique()
                    .HasDatabaseName("UX_clientes_correo");
            });

            // ============================================
            // ROLES
            // ============================================
            modelBuilder.Entity<Rol>(e =>
            {
                e.ToTable("roles");

                e.Property(r => r.Nombre)
                    .HasColumnName("nombre")
                    .HasMaxLength(40)
                    .IsRequired();

                e.Property(r => r.Descripcion)
                    .HasColumnName("descripcion")
                    .HasMaxLength(200);

                e.HasIndex(r => r.Nombre)
                    .IsUnique()
                    .HasDatabaseName("UX_roles_nombre");
            });
        }
    }
}
