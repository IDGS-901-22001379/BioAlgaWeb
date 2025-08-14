// Data/ApplicationDbContext.cs
using BioAlga.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles => Set<Rol>();
        public DbSet<Empleado> Empleados => Set<Empleado>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
        }
    }
}
