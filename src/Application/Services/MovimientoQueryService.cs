using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Fast_Bank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fast_Bank.Application.Services
{
    public class MovimientoQueryService
    {
        private readonly IDdContext _context;

        public MovimientoQueryService(IDdContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<MovimientoDto>> ObtenerTodosAsync()
        {
            var movimientos = await _context.Movimientos
                .Include(m => m.Origen)
                .Include(m => m.Destino)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();

            return movimientos.Select(MapToDto);
        }

        public async Task<MovimientoDto?> ObtenerPorIdAsync(string idMovimiento)
        {
            var movimiento = await _context.Movimientos
                .Include(m => m.Origen)
                .Include(m => m.Destino)
                .FirstOrDefaultAsync(m => m.IdMovimiento == idMovimiento);

            return movimiento != null ? MapToDto(movimiento) : null;
        }

        public async Task<IEnumerable<MovimientoDto>> ObtenerPorCuentaAsync(string numeroCuenta)
        {
            var movimientos = await _context.Movimientos
                .Include(m => m.Origen)
                .Include(m => m.Destino)
                .Where(m => m.Origen.NumeroCuenta == numeroCuenta || 
                           m.Destino.NumeroCuenta == numeroCuenta)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();

            return movimientos.Select(m => MapToDtoConContext(m, numeroCuenta));
        }

        private static MovimientoDto MapToDto(Movimiento movimiento)
        {
            return new MovimientoDto
            {
                IdMovimiento = movimiento.IdMovimiento,
                Tipo = movimiento.Tipo,
                Monto = movimiento.Monto,
                Fecha = movimiento.Fecha,
                Descripcion = movimiento.Descripcion,
                CuentaOrigen = movimiento.Origen?.NumeroCuenta,
                CuentaDestino = movimiento.Destino?.NumeroCuenta
            };
        }

        private static MovimientoDto MapToDtoConContext(Movimiento movimiento, string numeroCuenta)
        {
            var dto = MapToDto(movimiento);
            dto.EsDebito = movimiento.Origen?.NumeroCuenta == numeroCuenta;
            dto.EsCredito = movimiento.Destino?.NumeroCuenta == numeroCuenta;
            return dto;
        }
    }

    // DTO
    public class MovimientoDto
    {
        public string IdMovimiento { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string? CuentaOrigen { get; set; }
        public string? CuentaDestino { get; set; }
        public bool EsDebito { get; set; }
        public bool EsCredito { get; set; }
    }
}