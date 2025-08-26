using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using BioAlga.Backend.Data;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Services.Interfaces;

namespace BioAlga.Backend.Services
{
    public class CajaService : ICajaService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public CajaService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<int> AbrirCajaAsync(int idUsuario, CajaAperturaCreate req)
        {
            var activa = await _db.CajaAperturas
                .AnyAsync(a => a.IdUsuario == idUsuario && a.Activa);

            if (activa)
                throw new InvalidOperationException("Ya existe una apertura activa.");

            var apertura = new CajaApertura
            {
                FechaApertura = DateTime.UtcNow,
                IdUsuario = idUsuario,
                FondoInicial = req.FondoInicial,
                Activa = true
            };

            _db.CajaAperturas.Add(apertura);
            await _db.SaveChangesAsync();
            return apertura.IdCajaApertura;
        }

        public async Task<int> RegistrarMovimientoAsync(int idUsuario, CajaMovimientoCreate req)
        {
            var apertura = await _db.CajaAperturas
                .FirstOrDefaultAsync(a => a.IdCajaApertura == req.IdCajaApertura && a.Activa);

            if (apertura == null)
                throw new InvalidOperationException("La apertura no existe o está cerrada.");

            var mov = _mapper.Map<CajaMovimiento>(req);
            mov.Fecha = DateTime.UtcNow;
            mov.IdUsuario = idUsuario;

            _db.CajaMovimientos.Add(mov);
            await _db.SaveChangesAsync();
            return mov.IdCajaMovimiento;
        }

        public async Task<int> RealizarCorteAsync(int idUsuario, CajaCorteCreate req)
        {
            var apertura = await _db.CajaAperturas
                .FirstOrDefaultAsync(a => a.IdCajaApertura == req.IdCajaApertura && a.Activa);

            if (apertura == null)
                throw new InvalidOperationException("No hay apertura activa para este usuario.");

            var movs = await _db.CajaMovimientos
                .Where(m => m.IdCajaApertura == apertura.IdCajaApertura)
                .ToListAsync();

            var ingresos = movs.Where(m => m.Tipo == Models.Enums.TipoCajaMovimiento.Ingreso).Sum(m => m.MontoEfectivo);
            var egresos = movs.Where(m => m.Tipo == Models.Enums.TipoCajaMovimiento.Egreso).Sum(m => m.MontoEfectivo);

            var esperado = apertura.FondoInicial + ingresos - egresos;
            var diferencia = req.TotalEfectivoContado - esperado;

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var corte = new CajaCorte
                {
                    IdCajaApertura = apertura.IdCajaApertura,
                    FechaCorte = DateTime.UtcNow,
                    TotalEfectivoEsperado = esperado,
                    TotalEfectivoContado = req.TotalEfectivoContado,
                    Diferencia = diferencia,
                    IdUsuario = idUsuario
                };

                _db.CajaCortes.Add(corte);

                apertura.Activa = false;
                _db.CajaAperturas.Update(apertura);

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return corte.IdCajaCorte;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}
