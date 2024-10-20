using Microsoft.AspNetCore.Mvc;
using PuntoVentaBin.Shared.AccesoDatos;
using PuntoVentaBin.Shared.Identidades.Productos;
using PuntoVentaBin.Shared;
using Microsoft.EntityFrameworkCore;
using PuntoVentaBin.Shared.Identidades.Catalogos;

namespace PuntoVentaBin.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CatalogosController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        public CatalogosController(ApplicationDbContext context)
        {
            this.context = context;
        }

      
        [HttpPost("ValidarCodigoPostal")]
        public async Task<Respuesta<List<CodigosPostales>>> ValidarCodigoPostal([FromBody] string value)
        {
            var respuesta = new Respuesta<List<CodigosPostales>> { Estado = EstadosDeRespuesta.Correcto, Mensaje = "Facturando" };
            try
            {
                respuesta = await ValidarCodigoPostalMetodo(value).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al buscar el codigo postal {ex.Message}";
            }
            return respuesta;
        }


        public async Task<Respuesta<List<CodigosPostales>>> ValidarCodigoPostalMetodo(string value)
        {
            var respuesta = new Respuesta<List<CodigosPostales>>() { Estado = EstadosDeRespuesta.Correcto };
            var codigoPostalDB = context.Catalogo_Colonias.Any(cp => cp.c_CodigoPostal == value);
            if (!codigoPostalDB)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Datos = new List<CodigosPostales>();
                respuesta.Mensaje = $"No se encuentra el codigo postal";
                return respuesta;
            }

            var query = await (from cp in context.Catalogo_CodigosPostales
                               join e in context.Catalogo_Estados on cp.c_Estado equals e.c_Estado
                               where cp.c_CodigoPostal == value
                               select new { cp, e }).ToListAsync().ConfigureAwait(false);

            respuesta.Datos = query.Select(q => new CodigosPostales
            {
                c_CodigoPostal = q.cp.c_CodigoPostal,
                c_Estado = q.cp.c_Estado,
                c_Localidad = q.cp.c_Localidad,
                c_Municipio = q.cp.c_Municipio,
                Estado = new Estados
                {
                    c_Estado = q.e.c_Estado,
                    c_Pais = q.e.c_Pais,
                    Nombre = q.e.Nombre
                }
            }).ToList();

            respuesta.Datos.First().Municipio = await context.Catalogo_Municipios.FirstOrDefaultAsync(l => l.c_Municipio == respuesta.Datos.First().c_Municipio && l.c_Estado == respuesta.Datos.First().c_Estado).ConfigureAwait(false);

            respuesta.Datos.First().Localidad = await context.Catalogo_Localidades.FirstOrDefaultAsync(l => l.c_Localidad == respuesta.Datos.First().c_Localidad && l.c_Estado == respuesta.Datos.First().c_Estado).ConfigureAwait(false);

            respuesta.Datos.First().Colonias = await context.Catalogo_Colonias.Where(c => c.c_CodigoPostal == value).ToListAsync().ConfigureAwait(false);

            return respuesta;
        }
    }
}
