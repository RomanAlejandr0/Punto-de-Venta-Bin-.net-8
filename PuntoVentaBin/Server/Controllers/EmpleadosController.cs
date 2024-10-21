using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVentaBin.Shared;
using PuntoVentaBin.Shared.AccesoDatos;
using PuntoVentaBin.Shared.Identidades;

namespace PuntoVentaBin.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpleadosController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public EmpleadosController(ApplicationDbContext context)
        {
            this.context = context;
        }


        [HttpGet("{id}")]
        public async Task<Respuesta<List<Empleado>>> Get(long id)
        {
            var respuesta = new Respuesta<List<Empleado>> { Estado = EstadosDeRespuesta.Correcto };

            try
            {
                //respuesta.Datos = await context.Empleados.
                //    AsNoTracking().
                //    FirstOrDefaultAsync(x => x.usuarioId == id);

                respuesta.Datos = await context.Empleados.
                    Where(x => x.usuarioId == id).
                    OrderBy(x => x.Nombre).
                    ToListAsync();
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al consultar el usuario";
            }

            return respuesta;
        }
    }
}
