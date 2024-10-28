using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVentaBin.Shared;
using PuntoVentaBin.Shared.AccesoDatos;
using PuntoVentaBin.Shared.Identidades;
using PuntoVentaBin.Shared.Identidades.Productos;
using PuntoVentaBin.Shared.LogServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PuntoVentaBin.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly ILogService _logService;

        public CategoriasController(ApplicationDbContext context, ILogService logService)
        {
            this.context = context;
            _logService = logService;
        }

        [HttpGet("GetAll/{negocioId}")]
        public async Task<Respuesta<List<Categoria>>> GetAll(long negocioId)
        {
            var respuesta = new Respuesta<List<Categoria>> { Estado = EstadosDeRespuesta.Correcto };

            try
            {

                respuesta.Datos = await context.ProductoCategorias.
                    Where(x => x.NegocioId == negocioId).
                    OrderBy(x => x.Nombre).
                    AsNoTracking().
                    ToListAsync();
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al consultar las categorias";
            }

            return respuesta;
        }

        [HttpPost]
        [Route("{action}")]
        public async Task<Respuesta<long>> Guardar([FromBody] Categoria categoria)
        {
            var respuesta = new Respuesta<long> { Estado = EstadosDeRespuesta.Correcto };

            if (categoria.Id == 0)
            {
                respuesta = await GuardarCategoria(categoria);
            }
            else
            {
                respuesta = await EditarCategoria(categoria);
            }

            return respuesta;
        }

        public async Task<Respuesta<long>> GuardarCategoria(Categoria categoria)
        {
            var respuesta = new Respuesta<long> { Estado = EstadosDeRespuesta.Correcto };

            try
            {
                


                context.Add(categoria);
                await context.SaveChangesAsync();
                respuesta.Datos = categoria.Id;

                var objetoSerializado = JsonSerializer.Serialize(categoria);
                await _logService.LogAsync("Guardar Categoria", $"Nueva categoria: {objetoSerializado}", categoria.NegocioId);
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al guardar al guardar la categoria {categoria.Nombre}";
            }

            return respuesta;
        }

        public async Task<Respuesta<long>> EditarCategoria(Categoria categoria)
        {
            var respuesta = new Respuesta<long> { Estado = EstadosDeRespuesta.Correcto };

            try
            {
                var categoriaAnterior = await context.ProductoCategorias.AsNoTracking().FirstOrDefaultAsync(x => x.Id == categoria.Id);
                var objetoSerializadoAnterior = JsonSerializer.Serialize(categoriaAnterior);


                context.Attach(categoria).State = EntityState.Modified;
                await context.SaveChangesAsync();
                respuesta.Datos = categoria.Id;

                var objetoSerializado = JsonSerializer.Serialize(categoria);
                await _logService.LogAsync("Editar Categoria", $"Categoria editada de {objetoSerializadoAnterior} a {objetoSerializado}.", categoria.NegocioId);

            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al guardar los cambios {categoria.Nombre}";
            }

            return respuesta;
        }

        [HttpPost]
        [Route("{action}")]
        public async Task<Respuesta<long>> EliminarCategoria([FromBody] Categoria categoria)
        {
            var respuesta = new Respuesta<long> { Estado = EstadosDeRespuesta.Correcto, Mensaje = "Categoria eliminada correctamente" };

            try
            {
                context.Database.ExecuteSqlInterpolated($"UPDATE Productos SET Categoria = {null} WHERE Categoria = {categoria.Nombre}");
                context.Database.ExecuteSqlInterpolated($"DELETE FROM ProductoCategorias WHERE Id = {categoria.Id}");

                await context.SaveChangesAsync();

                var objetoSerializado = JsonSerializer.Serialize(categoria);
                await _logService.LogAsync("Eliminar categoria", $"Categoria Eliminada {objetoSerializado}.", categoria.NegocioId);
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al eliminar la categoria de base de datos.";
            }

            return respuesta;
        }
    }
}
