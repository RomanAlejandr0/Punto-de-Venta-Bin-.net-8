using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVentaBin.Server;
using PuntoVentaBin.Shared;
using PuntoVentaBin.Shared.AccesoDatos;
using PuntoVentaBin.Shared.Identidades;
using System.Diagnostics.Contracts;

namespace PuntoVentaBIN.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly ApplicationDbContext context;


        public PedidosController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet("{Id}")]
        public async Task<Respuesta<Venta>> Get(long id)
        {
            var respuesta = new Respuesta<Venta> { Estado = EstadosDeRespuesta.Correcto };

            var pedido = new Venta () { VentaDetalles = new List<VentaDetalle>() };

            try
            {
                pedido = await context.Ventas.
                    Include(x => x.VentaDetalles).
                    Include(x => x.PedidoExtension).
                    AsNoTracking().
                    FirstOrDefaultAsync(x => x.Id == id);

                respuesta.Datos = pedido;

            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al consultar el pedido";
            }

            return respuesta;
        }

        [HttpGet("GetPedidosActivos")]
        public async Task<Respuesta<List<Venta>>> GetPedidosActivos(long empresaId)
        {
            var respuesta = new Respuesta<List<Venta>> 
                { Estado = EstadosDeRespuesta.Correcto };

            var pedidos = new List<Venta>();

            try
            {
                //respuesta.Datos = await context.Pedidos
                    //.Where(x => x.EmpresaId == empresaId)
                    //.Include(c => c.PedidoDetalles)
                    //.ToListAsync();

                pedidos = await context.Ventas
                    .Where(x => x.EsPedido == true)
                    .AsNoTracking().ToListAsync();

                foreach (var pedido in pedidos)
                {
                    pedido.NombreCliente = await context.Clientes
                        .Where(x => x.Id == pedido.ClienteId)
                        .Select(x => x.Nombre)
                        .FirstOrDefaultAsync();
                    
                    pedido.PedidoExtension = await context.PedidosExtensiones
                        .Where(x => x.VentaId == pedido.Id)
                        .AsNoTracking().FirstOrDefaultAsync();

                    var pedidoDetalles = await context.VentaDetalles
                        .Where(pd => pd.VentaId == pedido.Id)
                        .ToListAsync();

                    pedido.VentaDetalles = pedidoDetalles;
                }

                respuesta.Datos = pedidos;
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al consultar los clientes";
            }

            return respuesta;
        }
    }
}
