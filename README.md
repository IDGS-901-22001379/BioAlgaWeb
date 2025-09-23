🛒 BioAlgaWeb – Sistema de Punto de Venta
📌 Descripción

BioAlgaWeb es un sistema de Punto de Venta (POS) desarrollado como proyecto académico/profesional para la gestión de una tienda.
Incluye administración completa de usuarios, empleados, clientes, proveedores, productos, compras, ventas, devoluciones, inventario y reportes, además de un dashboard interactivo con métricas clave.

El sistema está diseñado para ser modular, seguro y escalable, con soporte multi–rol y trazabilidad mediante auditoría y bitácora.

🚀 Funcionalidades principales

🔐 Seguridad: login con contraseñas cifradas, bloqueo por intentos fallidos, roles y permisos por módulo.

👥 Empleados: registro, inactivación ligada a usuarios, historial y reportes por actividad.

🧑‍🤝‍🧑 Clientes: CRUD, tipos de cliente (Normal, Mayoreo, Especial, Descuento) que determinan el precio aplicado en ventas.

📦 Productos: catálogo con SKU único, búsqueda por nombre/código de barras, tipos y categorías, validación de precios vigentes.

💲 Precios: gestión de precios multi–tier (Normal, Mayoreo, Especial, Descuento) con historial de vigencia.

🛍️ Compras: registro de compras, proveedores, costos históricos y aumento automático de inventario.

📊 Inventario: control exclusivo mediante movimientos (Entrada, Salida, Ajuste, Devolución), kardex y auditoría.

💳 Ventas POS: carrito de compras, múltiples métodos de pago, generación de tickets, descuentos controlados por rol.

🔄 Devoluciones y cancelaciones: reversión parcial o total de ventas con impacto en inventario.

💵 Caja y cortes: apertura de caja, ingresos/egresos, cortes por turno y diferencias.

📑 Pedidos: órdenes en grandes cantidades, reservas de stock y entregas parciales.

📈 Dashboard: KPIs de ventas, top clientes y productos, compras por proveedor, alertas de stock mínimo.

📝 Bitácora: auditoría completa de operaciones (logins, cambios de precios, ventas, ajustes, etc.).

📤 Importación/Exportación: carga inicial de productos/clientes/proveedores vía CSV, exportes a Excel/PDF.

🧑‍💻 Tecnologías utilizadas

Frontend: Angular + TypeScript, Bootstrap 5, Chart.js

Backend: ASP.NET Core 8 (C#) + Entity Framework Core

Base de datos: MySQL (tablas, vistas, procedimientos almacenados)

Herramientas: Swagger, Postman, Git/GitHub

Arquitectura: MVC, CQRS, Microservicios (intro)

Metodología: Scrum + prácticas DevOps básicas

📊 Dashboard y KPIs

Ventas por día, semana, mes, año

Top productos más vendidos

Top clientes por monto de compra

Compras por proveedor

Rotación de inventario y alertas de stock mínimo

📦 Instalación y ejecución

Clonar el repositorio:

git clone https://github.com/IDGS-901-22001379/BioAlgaWeb.git


Configurar la base de datos en MySQL con los scripts incluidos (/database).

Actualizar la cadena de conexión en appsettings.json.

Ejecutar backend:

dotnet run


Levantar frontend Angular:

ng serve -o

👥 Roles de usuario

👨‍💼 Administrador: acceso completo a todo el sistema.

📊 Gerencia: reportes avanzados, control de precios, cancelaciones fuera de día.

💳 Cajero/Vendedor: ventas, devoluciones parciales, tickets, descuentos limitados.

🛒 Compras: registro de proveedores y confirmación de compras.

📦 Inventario: ajustes, reservas, mermas y conteos de stock.

📚 Contabilidad (opcional): acceso a reportes de compras y ventas.

📸 Capturas

## 📸 Capturas de pantalla  

### 🔐 Login y Navegación  
| Login | Menú principal | Formulario |
|---|---|---|
| ![Login](web/Frontend/login.png) | ![Menú](web/Frontend/menu.png) | ![Formulario](web/Frontend/formulario.png) |

---

### 📊 Dashboard  
| Resumen general | Gráficas ventas | Top productos/clientes |
|---|---|---|
| ![Dashboard1](web/Frontend/dashboard1.png) | ![Dashboard2](web/Frontend/dashboard2.png) | ![Dashboard3](web/Frontend/dashboard3.png) |

---

### 📑 Pedidos  
| Lista de pedidos | Registro de pedido | Detalles de pedido |
|---|---|---|
| ![Pedidos1](web/Frontend/pedidos1.png) | ![Pedidos2](web/Frontend/pedidos2.png) | ![Pedidos3](web/Frontend/pedidos3.png) |

---

### 💳 Ventas  
| Punto de venta | Ticket generado | Listado de ventas |
|---|---|---|
| ![Ventas1](web/Frontend/ventas1.png) | ![Ventas2](web/Frontend/ventas2.png) | ![Ventas3](web/Frontend/ventas3.png) |

---

### 📦 Inventario  
| Lista de inventario | Movimientos (Entradas/Salidas) |
|---|---|
| ![Inventario1](web/Frontend/inventario1.png) | ![Inventario2](web/Frontend/inventario2.png) |

---

### 🛒 Compras  
| Lista de compras | Registro de compra | Detalles de compra |
|---|---|---|
| ![Compras1](web/Frontend/compras1.png) | ![Compras2](web/Frontend/compras2.png) | ![Compras3](web/Frontend/compras3.png) |


📌 Autor

👤 Yael López Mariano
Desarrollador Full Stack – Proyecto BioAlgaWeb Punto de Venta
