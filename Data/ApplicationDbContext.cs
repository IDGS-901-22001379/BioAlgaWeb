// Data/ApplicationDbContext.cs
using BioAlga.Backend.Models;
using Microsoft.EntityFrameworkCore;
using BioAlga.Backend.Models.Dashboard;
using BioAlga.Backend.Models.Enums; // EstatusPedido, etc.

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

        // ======= Ventas  =======
        public DbSet<Venta> Ventas { get; set; } = null!;
        public DbSet<DetalleVenta> DetalleVentas { get; set; } = null!;

        // ======= DEVOLUCIONES =======
        public DbSet<Devolucion> Devoluciones => Set<Devolucion>();
        public DbSet<DetalleDevolucion> DetalleDevoluciones => Set<DetalleDevolucion>();

        // ======= PEDIDOS  =======
        public DbSet<Pedido> Pedidos => Set<Pedido>();
        public DbSet<DetallePedido> DetallePedidos => Set<DetallePedido>();

        // ======= CORTE DE CAJA =======
        public DbSet<Caja> Cajas { get; set; } = null!;
        public DbSet<CajaTurno> CajaTurnos { get; set; } = null!;
        public DbSet<CajaMovimiento> CajaMovimientos { get; set; } = null!;
        public DbSet<VentaPago> VentaPagos { get; set; } = null!;



        // ======= DASHBOARD (vistas SQL) =======
        public DbSet<VentasResumen> VentasResumen => Set<VentasResumen>();
        public DbSet<TopProducto> TopProductos => Set<TopProducto>();
        public DbSet<TopCliente> TopClientes => Set<TopCliente>();
        public DbSet<VentasPorUsuario> VentasPorUsuarios => Set<VentasPorUsuario>();
        public DbSet<DevolucionesPorUsuario> DevolucionesPorUsuarios => Set<DevolucionesPorUsuario>();
        public DbSet<ComprasPorProveedor> ComprasPorProveedores => Set<ComprasPorProveedor>();

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
                e.Property(m => m.IdUsuario).HasColumnName("idUsuario").HasColumnName("id_usuario");
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
            // DEVOLUCIONES
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

                e.Property(x => x.VentaId).HasColumnName("venta_id");
                e.HasOne(x => x.Venta)
                    .WithMany()
                    .HasForeignKey(x => x.VentaId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_devol_venta");

                e.HasOne(x => x.Usuario)
                    .WithMany()
                    .HasForeignKey(x => x.IdUsuario)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("fk_devol_usuario");

                e.HasMany(x => x.Detalles)
                    .WithOne(d => d.Devolucion!)
                    .HasForeignKey(d => d.IdDevolucion)
                    .OnDelete(DeleteBehavior.Cascade);

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

                e.Property(x => x.IdDetalleVenta).HasColumnName("id_detalle_venta");
                e.HasOne(x => x.DetalleVenta)
                    .WithMany()
                    .HasForeignKey(x => x.IdDetalleVenta)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_dd_detventa");

                e.HasOne(d => d.Producto)
                    .WithMany()
                    .HasForeignKey(d => d.IdProducto)
                    .OnDelete(DeleteBehavior.NoAction);

                e.HasIndex(x => x.IdDevolucion).HasDatabaseName("idx_dd_devolucion");
                e.HasIndex(x => x.IdProducto).HasDatabaseName("idx_dd_producto");
            });


            // ============================================
            // PEDIDOS (NUEVO)
            // ============================================
            modelBuilder.Entity<Pedido>(e =>
            {
                e.ToTable("pedidos");

                e.HasKey(p => p.IdPedido);
                e.Property(p => p.IdPedido).HasColumnName("id_pedido");

                e.Property(p => p.IdCliente).HasColumnName("cliente_id").IsRequired();
                e.Property(p => p.IdUsuario).HasColumnName("id_usuario").IsRequired();

                e.Property(p => p.FechaPedido).HasColumnName("fecha_pedido");
                e.Property(p => p.FechaRequerida).HasColumnName("fecha_requerida");

                e.Property(p => p.Anticipo).HasColumnName("anticipo").HasColumnType("decimal(12,2)");
                e.Property(p => p.Subtotal).HasColumnName("subtotal").HasColumnType("decimal(12,2)");
                e.Property(p => p.Impuestos).HasColumnName("impuestos").HasColumnType("decimal(12,2)");
                e.Property(p => p.Total).HasColumnName("total").HasColumnType("decimal(12,2)");

                e.Property(p => p.Estatus).HasColumnName("estatus").HasConversion<string>();
                e.Property(p => p.Notas).HasColumnName("notas");

                e.HasIndex(p => p.Estatus).HasDatabaseName("idx_pedido_estatus");

                e.HasOne(p => p.Cliente).WithMany()
                    .HasForeignKey(p => p.IdCliente)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("fk_pedido_cliente");

                e.HasOne(p => p.Usuario).WithMany()
                    .HasForeignKey(p => p.IdUsuario)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("fk_pedido_usuario");

                e.HasMany(p => p.Detalles).WithOne(d => d.Pedido!)
                    .HasForeignKey(d => d.IdPedido)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DetallePedido>(e =>
            {
                e.ToTable("detalle_pedido");

                e.HasKey(d => d.IdDetalle);
                e.Property(d => d.IdDetalle).HasColumnName("id_detalle");

                e.Property(d => d.IdPedido).HasColumnName("id_pedido").IsRequired();
                e.Property(d => d.IdProducto).HasColumnName("id_producto").IsRequired();

                e.Property(d => d.Cantidad).HasColumnName("cantidad").IsRequired();
                e.Property(d => d.PrecioUnitario).HasColumnName("precio_unitario").HasColumnType("decimal(10,2)").IsRequired();

                e.HasOne(d => d.Producto).WithMany()
                    .HasForeignKey(d => d.IdProducto)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("fk_dpedido_prod");
            });
            // ============================================
            // DASHBOARD - mapeo de vistas (Keyless)
            // ============================================

            // ========= Ventas Resumen =========
            modelBuilder.Entity<VentasResumen>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("vw_ventas_resumen");
            });

            // ========= Top Productos por ingreso =========
            modelBuilder.Entity<TopProducto>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("vw_top_productos_ingreso");
                entity.Property(e => e.IdProducto).HasColumnName("id_producto");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.TotalUnidades).HasColumnName("total_unidades");
                entity.Property(e => e.IngresoTotal).HasColumnName("ingreso_total");
            });

            // ========= Top Clientes =========
            modelBuilder.Entity<TopCliente>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("vw_top_clientes");
                entity.Property(e => e.IdCliente).HasColumnName("cliente_id");
                entity.Property(e => e.NombreCompleto).HasColumnName("nombre_completo");
                entity.Property(e => e.TotalGastado).HasColumnName("total_gastado");
            });

            // ========= Ventas por Usuario =========
            modelBuilder.Entity<VentasPorUsuario>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("vw_ventas_por_usuario");
                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.ApellidoPaterno).HasColumnName("apellido_paterno");
                entity.Property(e => e.TotalVendido).HasColumnName("total_vendido");
                entity.Property(e => e.NumVentas).HasColumnName("num_ventas");
            });

            // ========= Devoluciones por Usuario =========
            modelBuilder.Entity<DevolucionesPorUsuario>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("vw_devoluciones_por_usuario");
                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
                entity.Property(e => e.NombreUsuario).HasColumnName("nombre_usuario");
                entity.Property(e => e.NumDevoluciones).HasColumnName("num_devoluciones");
                entity.Property(e => e.TotalDevuelto).HasColumnName("total_devuelto");
            });

            // ========= Compras por Proveedor =========
            modelBuilder.Entity<ComprasPorProveedor>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("vw_compras_por_proveedor");
                entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
                entity.Property(e => e.NombreEmpresa).HasColumnName("nombre_empresa");
                entity.Property(e => e.TotalComprado).HasColumnName("total_comprado");
                entity.Property(e => e.NumCompras).HasColumnName("num_compras");
            });

            // ============================================
            // CAJAS
            // ============================================
            modelBuilder.Entity<Caja>(e =>
            {
                e.ToTable("cajas");

                e.HasKey(x => x.IdCaja);
                e.Property(x => x.IdCaja).HasColumnName("id_caja");

                e.Property(x => x.Nombre)
                    .HasColumnName("nombre")
                    .HasMaxLength(50)
                    .IsRequired();

                e.Property(x => x.Descripcion)
                    .HasColumnName("descripcion")
                    .HasMaxLength(150);

                e.HasIndex(x => x.Nombre).IsUnique();
            });

            // ============================================
            // CAJA_TURNOS (apertura / cierre)
            // ============================================
            modelBuilder.Entity<CajaTurno>(e =>
            {
                e.ToTable("caja_turnos");

                e.HasKey(x => x.IdTurno);
                e.Property(x => x.IdTurno).HasColumnName("id_turno");

                e.Property(x => x.IdCaja).HasColumnName("id_caja").IsRequired();
                e.Property(x => x.IdUsuario).HasColumnName("id_usuario").IsRequired();

                e.Property(x => x.Apertura)
                    .HasColumnName("apertura")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                e.Property(x => x.Cierre)
                    .HasColumnName("cierre");

                e.Property(x => x.SaldoInicial)
                    .HasColumnName("saldo_inicial")
                    .HasColumnType("decimal(12,2)")
                    .HasDefaultValue(0);

                e.Property(x => x.SaldoCierre)
                    .HasColumnName("saldo_cierre")
                    .HasColumnType("decimal(12,2)");

                e.Property(x => x.Observaciones)
                    .HasColumnName("observaciones");

                e.HasOne(x => x.Caja)
                    .WithMany()
                    .HasForeignKey(x => x.IdCaja)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_turno_caja");

                e.HasOne<Usuario>()
                    .WithMany()
                    .HasForeignKey(x => x.IdUsuario)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("fk_turno_user");

                e.HasIndex(x => new { x.IdCaja, x.Apertura, x.Cierre })
                    .HasDatabaseName("idx_turno_caja_fecha");

                e.HasIndex(x => new { x.IdUsuario, x.Apertura, x.Cierre })
                    .HasDatabaseName("idx_turno_user_fecha");
            });

            // ============================================
            // CAJA_MOVIMIENTOS (Entradas / Salidas)
            // ============================================
            modelBuilder.Entity<CajaMovimiento>(e =>
            {
                e.ToTable("caja_movimientos");

                e.HasKey(x => x.IdMov);
                e.Property(x => x.IdMov).HasColumnName("id_mov");

                e.Property(x => x.IdTurno).HasColumnName("id_turno").IsRequired();

                e.Property(x => x.Fecha)
                    .HasColumnName("fecha")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // 'Ingreso' | 'Egreso' (guardado como string)
                e.Property(x => x.Tipo)
                    .HasColumnName("tipo")
                    .HasMaxLength(10)
                    .IsRequired();

                e.Property(x => x.Concepto)
                    .HasColumnName("concepto")
                    .HasMaxLength(150)
                    .IsRequired();

                e.Property(x => x.Monto)
                    .HasColumnName("monto")
                    .HasColumnType("decimal(12,2)")
                    .IsRequired();

                e.Property(x => x.Referencia)
                    .HasColumnName("referencia")
                    .HasMaxLength(100);

                e.HasOne(x => x.Turno)
                    .WithMany()
                    .HasForeignKey(x => x.IdTurno)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_mov_turno");

                e.HasIndex(x => new { x.IdTurno, x.Fecha })
                    .HasDatabaseName("idx_mov_turno_fecha");

                e.HasIndex(x => x.Tipo)
                    .HasDatabaseName("idx_mov_tipo");
            });

            // ============================================
            // VENTA_PAGOS (desglose por método para Mixto)
            // ============================================
            modelBuilder.Entity<VentaPago>(e =>
            {
                e.ToTable("venta_pagos");

                e.HasKey(x => x.IdPago);
                e.Property(x => x.IdPago).HasColumnName("id_pago");

                e.Property(x => x.IdVenta).HasColumnName("id_venta").IsRequired();

                // 'Efectivo' | 'Tarjeta' | 'Transferencia' | 'Otro'
                e.Property(x => x.Metodo)
                    .HasColumnName("metodo")
                    .HasMaxLength(20)
                    .IsRequired();

                e.Property(x => x.Monto)
                    .HasColumnName("monto")
                    .HasColumnType("decimal(12,2)")
                    .IsRequired();

                e.Property(x => x.CreadoEn)
                    .HasColumnName("creado_en")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                e.HasOne<Venta>()
                    .WithMany()
                    .HasForeignKey(x => x.IdVenta)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_vp_venta");

                e.HasIndex(x => x.IdVenta).HasDatabaseName("idx_vp_venta");
                e.HasIndex(x => x.Metodo).HasDatabaseName("idx_vp_metodo");
            });






        }
    }
}
