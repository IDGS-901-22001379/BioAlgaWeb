// Data/ApplicationDbContext.cs
using BioAlga.Backend.Models;
using Microsoft.EntityFrameworkCore;
using BioAlga.Backend.Models.Enums; // (si usas enums de modelos)

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

        // ======= Catálogo =======
        public DbSet<Producto> Productos { get; set; } = null!;
        public DbSet<ProductoSpec> ProductoSpecs { get; set; } = null!;
        public DbSet<ProductoPrecio> ProductoPrecios { get; set; } = null!;

        // ======= Compras + Inventario =======
        public DbSet<Compra> Compras { get; set; } = null!;
        public DbSet<DetalleCompra> DetalleCompras { get; set; } = null!;
        public DbSet<InventarioMovimiento> InventarioMovimientos { get; set; } = null!;

        // ======= Ventas / Caja =======
        public DbSet<Venta> Ventas { get; set; } = null!;
        public DbSet<DetalleVenta> DetalleVentas { get; set; } = null!;
        public DbSet<CajaApertura> CajaAperturas { get; set; } = null!;
        public DbSet<CajaMovimiento> CajaMovimientos { get; set; } = null!;
        public DbSet<CajaCorte> CajaCortes { get; set; } = null!;

        // ======= DEVOLUCIONES (NUEVO) =======
        public DbSet<Devolucion> Devoluciones => Set<Devolucion>();
        public DbSet<DetalleDevolucion> DetalleDevoluciones => Set<DetalleDevolucion>();

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

                e.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
                e.Property(x => x.Apellido_Paterno).HasColumnName("apellido_paterno").HasMaxLength(100);
                e.Property(x => x.Apellido_Materno).HasColumnName("apellido_materno").HasMaxLength(100);
                e.Property(x => x.Curp).HasColumnName("curp").HasMaxLength(18);
                e.Property(x => x.Rfc).HasColumnName("rfc").HasMaxLength(13);
                e.Property(x => x.Correo).HasColumnName("correo").HasMaxLength(120);
                e.Property(x => x.Telefono).HasColumnName("telefono").HasMaxLength(20);
                e.Property(x => x.Puesto).HasColumnName("puesto").HasMaxLength(80);
                e.Property(x => x.Salario).HasColumnName("salario").HasColumnType("decimal(10,2)");
                e.Property(x => x.Fecha_Ingreso).HasColumnName("fecha_ingreso").HasColumnType("date");
                e.Property(x => x.Fecha_Baja).HasColumnName("fecha_baja").HasColumnType("date");
                e.Property(x => x.Estatus).HasColumnName("estatus").HasMaxLength(10).HasDefaultValue("Activo");
                e.Property(x => x.Created_At).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
                e.Property(x => x.Updated_At).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();
                e.HasIndex(x => new { x.Nombre, x.Apellido_Paterno, x.Apellido_Materno }).HasDatabaseName("idx_empleados_nombre");
                e.HasIndex(x => x.Correo).IsUnique(false).HasDatabaseName("idx_empleados_correo");
            });

            // ============================================
            // CLIENTES
            // ============================================
            modelBuilder.Entity<Cliente>(e =>
            {
                e.ToTable("clientes");

                e.Property(c => c.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
                e.Property(c => c.ApellidoPaterno).HasColumnName("apellido_paterno").HasMaxLength(100);
                e.Property(c => c.ApellidoMaterno).HasColumnName("apellido_materno").HasMaxLength(100);
                e.Property(c => c.Correo).HasColumnName("correo").HasMaxLength(100);
                e.Property(c => c.Telefono).HasColumnName("telefono").HasMaxLength(20);
                e.Property(c => c.Direccion).HasColumnName("direccion");
                e.Property(c => c.TipoCliente).HasColumnName("tipo_cliente").HasMaxLength(20);
                e.Property(c => c.Estado).HasColumnName("estado").HasMaxLength(20);
                e.Property(c => c.FechaRegistro).HasColumnName("fecha_registro");
                e.HasIndex(c => new { c.Nombre, c.ApellidoPaterno, c.ApellidoMaterno }).HasDatabaseName("idx_clientes_nombre");
                e.HasIndex(c => c.Correo).IsUnique().HasDatabaseName("UX_clientes_correo");
            });

            // ============================================
            // ROLES
            // ============================================
            modelBuilder.Entity<Rol>(e =>
            {
                e.ToTable("roles");
                e.Property(r => r.Nombre).HasColumnName("nombre").HasMaxLength(40).IsRequired();
                e.Property(r => r.Descripcion).HasColumnName("descripcion").HasMaxLength(200);
                e.HasIndex(r => r.Nombre).IsUnique().HasDatabaseName("UX_roles_nombre");
            });

            // ============================================
            // PROVEEDORES
            // ============================================
            modelBuilder.Entity<Proveedor>(e =>
            {
                e.ToTable("proveedores");

                e.HasKey(p => p.IdProveedor);
                e.Property(p => p.IdProveedor).HasColumnName("id_proveedor");
                e.Property(p => p.NombreEmpresa).HasColumnName("nombre_empresa").HasMaxLength(120).IsRequired();
                e.Property(p => p.Contacto).HasColumnName("contacto").HasMaxLength(100);
                e.Property(p => p.Correo).HasColumnName("correo").HasMaxLength(120);
                e.Property(p => p.Telefono).HasColumnName("telefono").HasMaxLength(20);
                e.Property(p => p.Direccion).HasColumnName("direccion");
                e.Property(p => p.Rfc).HasColumnName("rfc").HasMaxLength(13);
                e.Property(p => p.Pais).HasColumnName("pais").HasMaxLength(50);
                e.Property(p => p.Ciudad).HasColumnName("ciudad").HasMaxLength(50);
                e.Property(p => p.CodigoPostal).HasColumnName("codigo_postal").HasMaxLength(10);
                e.Property(p => p.Estatus).HasColumnName("estatus").HasMaxLength(10).HasDefaultValue("Activo");
                e.Property(p => p.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
                e.HasIndex(p => p.NombreEmpresa);
                e.HasIndex(p => new { p.Pais, p.Ciudad });
            });

            // ============================================
            // PRODUCTOS
            // ============================================
            modelBuilder.Entity<Producto>(e =>
            {
                e.ToTable("productos");

                e.HasKey(p => p.IdProducto);
                e.Property(p => p.IdProducto).HasColumnName("id_producto");
                e.Property(p => p.Nombre).HasColumnName("nombre").HasMaxLength(150).IsRequired();
                e.Property(p => p.Descripcion).HasColumnName("descripcion");
                e.Property(p => p.Tipo).HasColumnName("tipo").HasConversion<string>().HasMaxLength(20).IsRequired();
                e.Property(p => p.IdCategoria).HasColumnName("id_categoria");
                e.Property(p => p.IdMarca).HasColumnName("id_marca");
                e.Property(p => p.IdUnidad).HasColumnName("id_unidad");
                e.Property(p => p.ProveedorPreferenteId).HasColumnName("proveedor_preferente_id");
                e.Property(p => p.CodigoSku).HasColumnName("codigo_sku").HasMaxLength(40).IsRequired();
                e.Property(p => p.CodigoBarras).HasColumnName("codigo_barras").HasMaxLength(50);
                e.Property(p => p.Estatus).HasColumnName("estatus").HasMaxLength(10).HasDefaultValue("Activo").IsRequired();
                e.Property(p => p.Created_At).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
                e.Property(p => p.Updated_At).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();

                e.HasOne<Proveedor>().WithMany()
                    .HasForeignKey(p => p.ProveedorPreferenteId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_prod_prov");

                e.HasIndex(p => p.CodigoSku).IsUnique().HasDatabaseName("UX_prod_sku");
                e.HasIndex(p => p.CodigoBarras).IsUnique().HasDatabaseName("UX_prod_codigobarras");
                e.HasIndex(p => new { p.Nombre, p.CodigoSku, p.CodigoBarras }).HasDatabaseName("idx_prod_busqueda");
            });

            // ============================================
            // PRODUCTO SPECS
            // ============================================
            modelBuilder.Entity<ProductoSpec>(e =>
            {
                e.ToTable("producto_specs");
                e.HasKey(s => s.IdSpec);
                e.Property(s => s.IdSpec).HasColumnName("id_spec");
                e.Property(s => s.IdProducto).HasColumnName("id_producto");
                e.Property(s => s.Clave).HasColumnName("clave").HasMaxLength(80).IsRequired();
                e.Property(s => s.Valor).HasColumnName("valor").HasMaxLength(255).IsRequired();

                e.HasOne(s => s.Producto)
                    .WithMany(p => p.Especificaciones)
                    .HasForeignKey(s => s.IdProducto)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_specs_prod");
            });

            // ============================================
            // PRODUCTO PRECIOS
            // ============================================
            modelBuilder.Entity<ProductoPrecio>(e =>
            {
                e.ToTable("producto_precios");
                e.HasKey(pp => pp.IdPrecio);
                e.Property(pp => pp.IdPrecio).HasColumnName("id_precio");
                e.Property(pp => pp.IdProducto).HasColumnName("id_producto");
                e.Property(pp => pp.TipoPrecio).HasColumnName("tipo_precio").HasMaxLength(15).IsRequired();
                e.Property(pp => pp.Precio).HasColumnName("precio").HasColumnType("decimal(10,2)").IsRequired();
                e.Property(pp => pp.VigenteDesde).HasColumnName("vigente_desde").HasDefaultValueSql("CURRENT_TIMESTAMP");
                e.Property(pp => pp.VigenteHasta).HasColumnName("vigente_hasta");
                e.Property(pp => pp.Activo).HasColumnName("activo").HasDefaultValue(true).IsRequired();

                e.HasOne(pp => pp.Producto)
                    .WithMany(p => p.Precios)
                    .HasForeignKey(pp => pp.IdProducto)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_precio_prod");

                e.HasIndex(pp => new { pp.IdProducto, pp.TipoPrecio, pp.Activo, pp.VigenteDesde })
                    .HasDatabaseName("idx_precio_vigencia");
            });

            // ============================================
            // COMPRAS
            // ============================================
            modelBuilder.Entity<Compra>(e =>
            {
                e.ToTable("compras");
                e.HasKey(c => c.IdCompra);
                e.Property(c => c.IdCompra).HasColumnName("id_compra");
                e.Property(c => c.ProveedorId).HasColumnName("proveedor_id");
                e.Property(c => c.ProveedorTexto).HasColumnName("proveedor_texto");
                e.Property(c => c.FechaCompra).HasColumnName("fecha_compra");
                e.Property(c => c.Subtotal).HasColumnName("subtotal").HasColumnType("decimal(12,2)");
                e.Property(c => c.Impuestos).HasColumnName("impuestos").HasColumnType("decimal(12,2)");
                e.Property(c => c.Total).HasColumnName("total").HasColumnType("decimal(12,2)");
                e.Property(c => c.IdUsuario).HasColumnName("id_usuario");
                e.Property(c => c.Notas).HasColumnName("notas");

                e.HasOne<Proveedor>().WithMany()
                    .HasForeignKey(c => c.ProveedorId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("fk_compra_prov");

                e.HasOne<Usuario>().WithMany()
                    .HasForeignKey(c => c.IdUsuario)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("fk_compra_user");

                e.HasIndex(c => c.FechaCompra).HasDatabaseName("idx_compra_fecha");
                e.HasIndex(c => c.ProveedorId).HasDatabaseName("idx_compra_proveedor");
            });

            modelBuilder.Entity<DetalleCompra>(e =>
            {
                e.ToTable("detalle_compra");
                e.HasKey(d => d.IdDetalle);
                e.Property(d => d.IdDetalle).HasColumnName("id_detalle");
                e.Property(d => d.IdCompra).HasColumnName("id_compra");
                e.Property(d => d.IdProducto).HasColumnName("id_producto");
                e.Property(d => d.Cantidad).HasColumnName("cantidad");
                e.Property(d => d.CostoUnitario).HasColumnName("costo_unitario").HasColumnType("decimal(10,2)");
                e.Property(d => d.IvaUnitario).HasColumnName("iva_unitario").HasColumnType("decimal(10,2)");

                e.HasOne(d => d.Compra).WithMany(c => c.Detalles)
                    .HasForeignKey(d => d.IdCompra)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_dcompra_compra");

                e.HasOne(d => d.Producto).WithMany()
                    .HasForeignKey(d => d.IdProducto)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("fk_dcompra_prod");
            });

            // ============================================
            // INVENTARIO_MOVIMIENTOS
            // ============================================
            modelBuilder.Entity<InventarioMovimiento>(e =>
            {
                e.ToTable("inventario_movimientos");
                e.HasKey(m => m.IdMovimiento);
                e.Property(m => m.IdMovimiento).HasColumnName("id_movimiento");
                e.Property(m => m.IdProducto).HasColumnName("id_producto");
                e.Property(m => m.TipoMovimiento).HasColumnName("tipo_movimiento").HasMaxLength(10);
                e.Property(m => m.Cantidad).HasColumnName("cantidad");
                e.Property(m => m.Fecha).HasColumnName("fecha").HasDefaultValueSql("CURRENT_TIMESTAMP");
                e.Property(m => m.OrigenTipo).HasColumnName("origen_tipo").HasMaxLength(20);
                e.Property(m => m.OrigenId).HasColumnName("origen_id");
                e.Property(m => m.IdUsuario).HasColumnName("id_usuario");
                e.Property(m => m.Referencia).HasColumnName("referencia");
                e.HasIndex(m => new { m.OrigenTipo, m.OrigenId }).HasDatabaseName("idx_mov_origen");
                e.HasIndex(m => new { m.IdProducto, m.Fecha }).HasDatabaseName("idx_mov_prod_fecha");
            });

            // ============================================
            // VENTAS
            // ============================================
            modelBuilder.Entity<Venta>(e =>
            {
                e.HasKey(x => x.IdVenta);
                e.Property(x => x.MetodoPago).HasConversion<string>();
                e.Property(x => x.Estatus).HasConversion<string>();

                e.HasOne(v => v.Cliente).WithMany()
                    .HasForeignKey(v => v.ClienteId)
                    .HasConstraintName("fk_venta_cliente");

                e.HasOne(v => v.Usuario).WithMany()
                    .HasForeignKey(v => v.IdUsuario)
                    .HasConstraintName("fk_venta_usuario");

                e.HasMany(v => v.Detalles).WithOne(d => d.Venta!)
                    .HasForeignKey(d => d.IdVenta)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_dventa_venta");
            });

            modelBuilder.Entity<DetalleVenta>(e =>
            {
                e.HasKey(x => x.IdDetalle);
                e.HasOne(d => d.Producto).WithMany()
                    .HasForeignKey(d => d.IdProducto)
                    .HasConstraintName("fk_dventa_prod");
            });

            // ============================================
            // DEVOLUCIONES (ÚNICO BLOQUE NUEVO/ACTUALIZADO)
            // ============================================
            modelBuilder.Entity<Devolucion>(e =>
            {
                e.ToTable("devoluciones");

                e.HasKey(x => x.IdDevolucion);
                e.Property(x => x.IdDevolucion).HasColumnName("id_devolucion");

                e.Property(x => x.FechaDevolucion)
                    .HasColumnName("fecha_devolucion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                e.Property(x => x.IdUsuario).HasColumnName("id_usuario");

                e.Property(x => x.UsuarioNombre)
                    .HasColumnName("usuario_nombre")
                    .HasMaxLength(120)
                    .IsRequired();

                e.Property(x => x.Motivo)
                    .HasColumnName("motivo")
                    .HasMaxLength(300)
                    .IsRequired();

                e.Property(x => x.RegresaInventario)
                    .HasColumnName("regresa_inventario")
                    .IsRequired();

                e.Property(x => x.TotalDevuelto)
                    .HasColumnName("total_devuelto")
                    .HasColumnType("decimal(12,2)")
                    .IsRequired();

                e.Property(x => x.ReferenciaVenta)
                    .HasColumnName("referencia_venta")
                    .HasMaxLength(50);

                // --- FK opcional a venta (si tienes columna venta_id) ---
                e.Property(x => x.VentaId).HasColumnName("venta_id");
                e.HasOne(x => x.Venta)
                    .WithMany()
                    .HasForeignKey(x => x.VentaId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_devol_venta");

                // FK Usuario (integridad)
                e.HasOne(x => x.Usuario)
                    .WithMany()
                    .HasForeignKey(x => x.IdUsuario)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("fk_devol_usuario");

                // Relación con detalles
                e.HasMany(x => x.Detalles)
                    .WithOne(d => d.Devolucion!)
                    .HasForeignKey(d => d.IdDevolucion)
                    .OnDelete(DeleteBehavior.Cascade);

                // Índices
                e.HasIndex(x => x.FechaDevolucion).HasDatabaseName("idx_devol_fecha");
                e.HasIndex(x => x.IdUsuario).HasDatabaseName("idx_devol_usuario");
            });

            modelBuilder.Entity<DetalleDevolucion>(e =>
            {
                e.ToTable("detalle_devolucion");

                e.HasKey(x => x.IdDetalle);
                e.Property(x => x.IdDetalle).HasColumnName("id_detalle");

                e.Property(x => x.IdDevolucion).HasColumnName("id_devolucion");
                e.Property(x => x.IdProducto).HasColumnName("id_producto");

                e.Property(x => x.ProductoNombre)
                    .HasColumnName("producto_nombre")
                    .HasMaxLength(150)
                    .IsRequired();

                e.Property(x => x.Cantidad)
                    .HasColumnName("cantidad")
                    .IsRequired();

                e.Property(x => x.ImporteLineaTotal)
                    .HasColumnName("importe_linea_total")
                    .HasColumnType("decimal(12,2)")
                    .IsRequired();

                // --- FK opcional al renglón de venta (si tienes columna id_detalle_venta) ---
                e.Property(x => x.IdDetalleVenta).HasColumnName("id_detalle_venta");
                e.HasOne(x => x.DetalleVenta)
                    .WithMany()
                    .HasForeignKey(x => x.IdDetalleVenta)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_dd_detventa");

                // Navegación a producto (opcional)
                e.HasOne(d => d.Producto)
                    .WithMany()
                    .HasForeignKey(d => d.IdProducto)
                    .OnDelete(DeleteBehavior.NoAction);

                e.HasIndex(x => x.IdDevolucion).HasDatabaseName("idx_dd_devolucion");
                e.HasIndex(x => x.IdProducto).HasDatabaseName("idx_dd_producto");
            });

            // ============================================
            // CAJA
            // ============================================
            modelBuilder.Entity<CajaApertura>(e =>
            {
                e.ToTable("caja_aperturas");
                e.HasKey(x => x.IdCajaApertura);
                e.Property(x => x.IdCajaApertura).HasColumnName("id_caja_apertura");
                e.Property(x => x.FechaApertura).HasColumnName("fecha_apertura");
                e.Property(x => x.IdUsuario).HasColumnName("id_usuario");
                e.Property(x => x.FondoInicial).HasColumnName("fondo_inicial").HasColumnType("decimal(12,2)");
                e.Property(x => x.Activa).HasColumnName("activa");
            });

            modelBuilder.Entity<CajaMovimiento>(e =>
            {
                e.ToTable("caja_movimientos");
                e.HasKey(x => x.IdCajaMovimiento);
                e.Property(x => x.IdCajaMovimiento).HasColumnName("id_caja_movimiento");
                e.Property(x => x.IdCajaApertura).HasColumnName("id_caja_apertura");
                e.Property(x => x.Fecha).HasColumnName("fecha");
                e.Property(x => x.Tipo).HasConversion<string>().HasColumnName("tipo");
                e.Property(x => x.Concepto).HasColumnName("concepto").HasMaxLength(180);
                e.Property(x => x.MontoEfectivo).HasColumnName("monto_efectivo").HasColumnType("decimal(12,2)");
                e.Property(x => x.IdVenta).HasColumnName("id_venta");
                e.Property(x => x.IdUsuario).HasColumnName("id_usuario");

                e.HasOne(x => x.CajaApertura).WithMany().HasForeignKey(x => x.IdCajaApertura);
                e.HasOne(x => x.Venta).WithMany().HasForeignKey(x => x.IdVenta).OnDelete(DeleteBehavior.SetNull);

                e.HasIndex(x => x.Fecha).HasDatabaseName("idx_caja_mov_fecha");
            });

            modelBuilder.Entity<CajaCorte>(e =>
            {
                e.ToTable("caja_cortes");
                e.HasKey(x => x.IdCajaCorte);
                e.Property(x => x.IdCajaCorte).HasColumnName("id_caja_corte");
                e.Property(x => x.IdCajaApertura).HasColumnName("id_caja_apertura");
                e.Property(x => x.FechaCorte).HasColumnName("fecha_corte");
                e.Property(x => x.TotalEfectivoEsperado).HasColumnName("total_efectivo_esperado").HasColumnType("decimal(12,2)");
                e.Property(x => x.TotalEfectivoContado).HasColumnName("total_efectivo_contado").HasColumnType("decimal(12,2)");
                e.Property(x => x.Diferencia).HasColumnName("diferencia").HasColumnType("decimal(12,2)");
                e.Property(x => x.IdUsuario).HasColumnName("id_usuario");

                e.HasOne(x => x.CajaApertura).WithMany().HasForeignKey(x => x.IdCajaApertura);
            });

            // (Si después mapeas vistas, usa entidades keyless con .ToView(...))
        }
    }
}
