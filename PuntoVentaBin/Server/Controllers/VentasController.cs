using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVentaBin.Shared;
using PuntoVentaBin.Shared.AccesoDatos;
using PuntoVentaBin.Shared.Identidades;
using PuntoVentaBin.Shared.Identidades.DTOs;
using PuntoVentaBin.Shared.LogServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PuntoVentaBin.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly ApplicationDbContext context;

        public VentasController(ApplicationDbContext context, ILogService logService)
        {
            _logService = logService;
            this.context = context;
        }

        [HttpPost("GetVentas")]
        public async Task<Respuesta<List<Venta>>> GetVentas(FechaInicioFechaFin periodoFecha)
        {
            var respuesta = new Respuesta<List<Venta>> { Estado = EstadosDeRespuesta.Correcto };

            List<Venta> Ventas = new List<Venta>();

            var tsInicio = new TimeSpan(00, 00, 01);
            periodoFecha.FechaInicio = periodoFecha.FechaInicio.Date + tsInicio;

            var tsFin = new TimeSpan(23, 59, 59);
            periodoFecha.FechaFin = periodoFecha.FechaFin.Date + tsFin;

            try
            {
                Ventas = await context.Ventas.
                    Where(x => x.NegocioId == periodoFecha.NegocioId &&
                        x.FechaHoraVenta >= periodoFecha.FechaInicio &&
                        x.FechaHoraVenta <= periodoFecha.FechaFin).
                    OrderByDescending(x => x.FechaHoraVenta).
                    AsNoTracking().
                    ToListAsync();

                respuesta.Datos = Ventas;

            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al consultar las ventas";
            }

            return respuesta;
        }

        [HttpGet("GetVentaDetalles/{ventaId}")]
        public async Task<Respuesta<List<VentaDetalle>>> GetVentaDetalles(long ventaId)
        {
            var respuesta = new Respuesta<List<VentaDetalle>> { Estado = EstadosDeRespuesta.Correcto };

            try
            {
                respuesta.Datos = await context.VentaDetalles.
                    Where(x => x.VentaId == ventaId).
                    AsNoTracking().
                    ToListAsync();
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al consultar las ventas {ex.InnerException}";
            }

            return respuesta;
        }

        [HttpPost]
        [Route("{action}")]
        public async Task<Respuesta<long>> GuardarVenta([FromBody] Venta venta)
        {
            var respuesta = new Respuesta<long>
            {
                Estado = EstadosDeRespuesta.Correcto,
                Mensaje = "Venta Guardada Correctamente"
            };

            try
            {

                context.Ventas.Add(venta);
                await context.SaveChangesAsync();

                foreach (var ventaDetalle in venta.VentaDetalles)
                {
                    var producto = await context.Productos.
                        Where(x => x.Id == ventaDetalle.ProductoId).
                        FirstOrDefaultAsync();

                    producto.CantidadInventario -= ventaDetalle.Cantidad;
                    await context.SaveChangesAsync();

                }

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    WriteIndented = true // Opcional: para una salida más legible
                };


                var ventaSerializada = JsonSerializer.Serialize(venta, options);
                await _logService.LogAsync("Guardar venta", $"Nuevo venta: {ventaSerializada}", venta.NegocioId);

                //var nombreUsuario = await context.Usuarios.Where(x => x.Id == venta.UsuarioId).Select(x => x.Nombre).FirstOrDefaultAsync();

                //var Movimiento = new Movimiento()
                //{
                //    NombreUsuario = nombreUsuario,
                //    Mensaje = $"El usuario Id: {venta.UsuarioId} Nombre: {nombreUsuario} realizo una venta el {venta.FechaHoraVenta} " +
                //    $"Total de productos vendidos: {venta.VentaDetalles.Count} con un valor de: {venta.Total}"
                //};

                //context.Movimientos.Add(Movimiento);
                //await context.SaveChangesAsync().ConfigureAwait(false);

                respuesta.Datos = venta.Id;
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al guardar al guardar la venta Id: {venta.Id} Error: {ex.Message}";
            }

            return respuesta;
        }
        
    }
}
