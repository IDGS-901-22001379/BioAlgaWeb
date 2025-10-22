using System;
using System.Collections.Generic;

namespace BioAlga.Backend.Dtos
{
    public class CorteResumenDto
    {
        // Turno
        public int Id_Turno { get; set; }
        public int Id_Caja { get; set; }
        public int Id_Usuario { get; set; }
        public DateTime Apertura { get; set; }
        public DateTime Cierre_Usado { get; set; } // si turno sigue abierto, se usa DateTime.UtcNow

        // Ventas
        public int Num_Tickets { get; set; }
        public decimal Ventas_Totales { get; set; }

        // Métodos de pago (si hay venta_pagos se usa eso; si no, se agrupa por ventas.metodo_pago)
        public List<CorteMetodoDto> Ventas_Por_Metodo { get; set; } = new();

        // Efectivo de ventas según tickets: (efectivo_recibido - cambio)
        public decimal Ventas_Efectivo_Neto { get; set; }

        // Movimientos manuales
        public decimal Entradas_Efectivo { get; set; }
        public decimal Salidas_Efectivo { get; set; }

        // Devoluciones (monto total)
        public decimal Devoluciones_Total { get; set; }

        // Fondo
        public decimal Saldo_Inicial { get; set; }

        // Dinero esperado en caja (arqueo teórico)
        public decimal Efectivo_Esperado { get; set; }
    }
}
