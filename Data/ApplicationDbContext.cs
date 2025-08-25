// Data/ApplicationDbContext.cs
using BioAlga.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // ======= DbSets existentes =======
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Rol> Roles { get; set; } = null!;
        public DbSet<Empleado> Empleados { get; set; } = null!;
        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<Proveedor> Proveedores { get; set; } = null!;

        // ======= NUEVOS DbSets (Catálogo) =======
        public DbSet<Producto> Productos { get; set; } = null!;
        public DbSet<ProductoSpec> ProductoSpecs { get; set; } = null!;
        public DbSet<ProductoPrecio> ProductoPrecios { get; set; } = null!;

        // ======= NUEVOS DbSets (Compras + Inventario) =======
        public DbSet<Compra> Compras { get; set; } = null!;
        public DbSet<DetalleCompra> DetalleCompras { get; set; } = null!;
        public DbSet<InventarioMovimiento> InventarioMovimientos { get; set; } = null!;


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

                e.HasOne(u => u.Rol)
                    .WithMany()
                    .HasForeignKey(u => u.Id_Rol)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_usuarios_roles");

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

                e.HasIndex(x => new { x.Nombre, x.Apellido_Paterno, x.Apellido_Materno })
                    .HasDatabaseName("idx_empleados_nombre");

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

            // ============================================
            // PROVEEDORES
            // ============================================
            modelBuilder.Entity<Proveedor>(e =>
            {
                e.ToTable("proveedores");

                e.HasKey(p => p.IdProveedor);
                e.Property(p => p.IdProveedor).HasColumnName("id_proveedor");

                e.Property(p => p.NombreEmpresa)
                    .HasColumnName("nombre_empresa")
                    .HasMaxLength(120)
                    .IsRequired();

                e.Property(p => p.Contacto)
                    .HasColumnName("contacto")
                    .HasMaxLength(100);

                e.Property(p => p.Correo)
                    .HasColumnName("correo")
                    .HasMaxLength(120);

                e.Property(p => p.Telefono)
                    .HasColumnName("telefono")
                    .HasMaxLength(20);

                e.Property(p => p.Direccion)
                    .HasColumnName("direccion");

                e.Property(p => p.Rfc)
                    .HasColumnName("rfc")
                    .HasMaxLength(13);

                e.Property(p => p.Pais)
                    .HasColumnName("pais")
                    .HasMaxLength(50);

                e.Property(p => p.Ciudad)
                    .HasColumnName("ciudad")
                    .HasMaxLength(50);

                e.Property(p => p.CodigoPostal)
                    .HasColumnName("codigo_postal")
                    .HasMaxLength(10);

                e.Property(p => p.Estatus)
                    .HasColumnName("estatus")
                    .HasMaxLength(10)
                    .HasDefaultValue("Activo");

                e.Property(p => p.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Índices útiles para búsqueda
                e.HasIndex(p => p.NombreEmpresa);
                e.HasIndex(p => new { p.Pais, p.Ciudad });
            });

            // ============================================
            // PRODUCTOS  (nuevo)
            // ============================================
            modelBuilder.Entity<Producto>(e =>
            {
                e.ToTable("productos");

                e.HasKey(p => p.IdProducto);
                e.Property(p => p.IdProducto).HasColumnName("id_producto");

                e.Property(p => p.Nombre)
                    .HasColumnName("nombre")
                    .HasMaxLength(150)
                    .IsRequired();

                e.Property(p => p.Descripcion)
                    .HasColumnName("descripcion");

                e.Property(p => p.Tipo)                 // enum → string
                    .HasColumnName("tipo")
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();

                e.Property(p => p.IdCategoria).HasColumnName("id_categoria");
                e.Property(p => p.IdMarca).HasColumnName("id_marca");
                e.Property(p => p.IdUnidad).HasColumnName("id_unidad");

                e.Property(p => p.ProveedorPreferenteId)
                    .HasColumnName("proveedor_preferente_id");

                e.Property(p => p.CodigoSku)
                    .HasColumnName("codigo_sku")
                    .HasMaxLength(40)
                    .IsRequired();

                e.Property(p => p.CodigoBarras)
                    .HasColumnName("codigo_barras")
                    .HasMaxLength(50);

                e.Property(p => p.Estatus)
                    .HasColumnName("estatus")
                    .HasMaxLength(10)
                    .HasDefaultValue("Activo")
                    .IsRequired();

                e.Property(p => p.Created_At)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                e.Property(p => p.Updated_At)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                // Relaciones
                e.HasOne<Proveedor>()   // proveedor preferente opcional
                    .WithMany()
                    .HasForeignKey(p => p.ProveedorPreferenteId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_prod_prov");

                // Índices (como en tu SQL)
                e.HasIndex(p => p.CodigoSku)
                    .IsUnique()
                    .HasDatabaseName("UX_prod_sku");

                e.HasIndex(p => p.CodigoBarras)
                    .IsUnique()
                    .HasDatabaseName("UX_prod_codigobarras");

                // Soporte a búsqueda rápida (nombre/sku/código de barras)
                e.HasIndex(p => new { p.Nombre, p.CodigoSku, p.CodigoBarras })
                    .HasDatabaseName("idx_prod_busqueda");
            });

            // ============================================
            // PRODUCTO SPECS  (nuevo)
            // ============================================
            modelBuilder.Entity<ProductoSpec>(e =>
            {
                e.ToTable("producto_specs");

                e.HasKey(s => s.IdSpec);
                e.Property(s => s.IdSpec).HasColumnName("id_spec");

                e.Property(s => s.IdProducto).HasColumnName("id_producto");

                e.Property(s => s.Clave)
                    .HasColumnName("clave")
                    .HasMaxLength(80)
                    .IsRequired();

                e.Property(s => s.Valor)
                    .HasColumnName("valor")
                    .HasMaxLength(255)
                    .IsRequired();

                e.HasOne(s => s.Producto)
                    .WithMany(p => p.Especificaciones)
                    .HasForeignKey(s => s.IdProducto)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_specs_prod");
            });

            // ============================================
            // PRODUCTO PRECIOS  (nuevo; histórico)
            // ============================================
            modelBuilder.Entity<ProductoPrecio>(e =>
            {
                e.ToTable("producto_precios");

                e.HasKey(pp => pp.IdPrecio);
                e.Property(pp => pp.IdPrecio).HasColumnName("id_precio");

                e.Property(pp => pp.IdProducto).HasColumnName("id_producto");

                e.Property(pp => pp.TipoPrecio)
                    .HasColumnName("tipo_precio")
                    .HasMaxLength(15) // Normal/Mayoreo/Descuento/Especial
                    .IsRequired();

                e.Property(pp => pp.Precio)
                    .HasColumnName("precio")
                    .HasColumnType("decimal(10,2)")
                    .IsRequired();

                e.Property(pp => pp.VigenteDesde)
                    .HasColumnName("vigente_desde")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                e.Property(pp => pp.VigenteHasta)
                    .HasColumnName("vigente_hasta");

                e.Property(pp => pp.Activo)
                    .HasColumnName("activo")
                    .HasDefaultValue(true)
                    .IsRequired();

                e.HasOne(pp => pp.Producto)
                    .WithMany(p => p.Precios)
                    .HasForeignKey(pp => pp.IdProducto)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_precio_prod");

                // Índice compuesto para vigencia (igual a tu script)
                e.HasIndex(pp => new { pp.IdProducto, pp.TipoPrecio, pp.Activo, pp.VigenteDesde })
                    .HasDatabaseName("idx_precio_vigencia");
            });

            // ============================================
            // COMPRAS (nuevo)
            // ============================================
            modelBuilder.Entity<Compra>(e =>
            {
                e.ToTable("compras");

                e.HasKey(c => c.IdCompra);
                e.Property(c => c.IdCompra).HasColumnName("id_compra");

                e.Property(c => c.ProveedorId).HasColumnName("proveedor_id");
                e.Property(c => c.ProveedorTexto).HasColumnName("proveedor_texto");

                e.Property(c => c.FechaCompra)
                    .HasColumnName("fecha_compra");

                e.Property(c => c.Subtotal)
                    .HasColumnName("subtotal")
                    .HasColumnType("decimal(12,2)");

                e.Property(c => c.Impuestos)
                    .HasColumnName("impuestos")
                    .HasColumnType("decimal(12,2)");

                e.Property(c => c.Total)
                    .HasColumnName("total")
                    .HasColumnType("decimal(12,2)");

                e.Property(c => c.IdUsuario).HasColumnName("id_usuario");
                e.Property(c => c.Notas).HasColumnName("notas");

                // FK proveedor (opcional)
                e.HasOne<Proveedor>()
                    .WithMany()
                    .HasForeignKey(c => c.ProveedorId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("fk_compra_prov");

                // FK usuario
                e.HasOne<Usuario>()
                    .WithMany()
                    .HasForeignKey(c => c.IdUsuario)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("fk_compra_user");

                // Índices de búsqueda (folio/proveedor/fecha)
                e.HasIndex(c => c.FechaCompra).HasDatabaseName("idx_compra_fecha");
                e.HasIndex(c => c.ProveedorId).HasDatabaseName("idx_compra_proveedor");
            });

            // ============================================
            // DETALLE_COMPRA (nuevo)
            // ============================================
            modelBuilder.Entity<DetalleCompra>(e =>
            {
                e.ToTable("detalle_compra");

                e.HasKey(d => d.IdDetalle);
                e.Property(d => d.IdDetalle).HasColumnName("id_detalle");

                e.Property(d => d.IdCompra).HasColumnName("id_compra");
                e.Property(d => d.IdProducto).HasColumnName("id_producto");

                e.Property(d => d.Cantidad)
                    .HasColumnName("cantidad");

                e.Property(d => d.CostoUnitario)
                    .HasColumnName("costo_unitario")
                    .HasColumnType("decimal(10,2)");

                e.Property(d => d.IvaUnitario)
                    .HasColumnName("iva_unitario")
                    .HasColumnType("decimal(10,2)");

                e.HasOne(d => d.Compra)
                    .WithMany(c => c.Detalles)
                    .HasForeignKey(d => d.IdCompra)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_dcompra_compra");

                e.HasOne(d => d.Producto)
                    .WithMany()
                    .HasForeignKey(d => d.IdProducto)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("fk_dcompra_prod");
            });

            // ============================================
            // INVENTARIO_MOVIMIENTOS (nuevo)
            // ============================================
            // Mapeo:
            modelBuilder.Entity<InventarioMovimiento>(e =>
            {
                e.ToTable("inventario_movimientos");

                e.HasKey(m => m.IdMovimiento);
                e.Property(m => m.IdMovimiento).HasColumnName("id_movimiento");

                e.Property(m => m.IdProducto).HasColumnName("id_producto");
                e.Property(m => m.TipoMovimiento)
                    .HasColumnName("tipo_movimiento")
                    .HasMaxLength(10); // Entrada/Salida/Ajuste

                e.Property(m => m.Cantidad).HasColumnName("cantidad");
                e.Property(m => m.Fecha)
                    .HasColumnName("fecha")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                e.Property(m => m.OrigenTipo)
                    .HasColumnName("origen_tipo")
                    .HasMaxLength(20);

                e.Property(m => m.OrigenId).HasColumnName("origen_id");
                e.Property(m => m.IdUsuario).HasColumnName("id_usuario");
                e.Property(m => m.Referencia).HasColumnName("referencia");

                e.HasIndex(m => new { m.OrigenTipo, m.OrigenId }).HasDatabaseName("idx_mov_origen");
                e.HasIndex(m => new { m.IdProducto, m.Fecha }).HasDatabaseName("idx_mov_prod_fecha");
            });

            // NOTA: si más adelante deseas mapear la vista vw_producto_precio_vigente,
            // crea una entidad keyless (HasNoKey) y usa ToView("vw_producto_precio_vigente").
        }
    }
}
