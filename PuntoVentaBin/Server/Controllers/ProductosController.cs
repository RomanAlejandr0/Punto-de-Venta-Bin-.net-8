﻿using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PuntoVentaBin.Shared;
using PuntoVentaBin.Shared.AccesoDatos;
using PuntoVentaBin.Shared.Identidades.Productos;
using PuntoVentaBin.Shared.Identidades.DTOs;
using System.Text.Json;
using PuntoVentaBin.Shared.LogServices;

namespace PuntoVentaBIN.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly ILogService _logService;


        public ProductosController(ApplicationDbContext context, ILogService logService)
        {
            this.context = context;
            _logService = logService;

        }

        [HttpGet("{id}")]
        public async Task<Respuesta<Producto>> Get(long id)
        {
            var respuesta = new Respuesta<Producto>
            {
                Estado = EstadosDeRespuesta.Correcto,
                Mensaje = "Operacion exitosa"
            };

            try
            {
                var producto = await context.Productos.
                    //Include(x => x.ProductosPaquete).
                    AsNoTracking().
                    FirstOrDefaultAsync(x => x.Id == id);

                respuesta.Datos = producto;
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al consultar el producto {ex.InnerException}";
            }

            return respuesta;
        }

        [HttpGet("GetAll/{negocioId}")]
        public async Task<Respuesta<List<Producto>>> GetAll(long negocioId)
        {
            var respuesta = new Respuesta<List<Producto>>
            {
                Estado = EstadosDeRespuesta.Correcto,
                Mensaje = "Operacion exitosa"
            };

            try
            {
                var productos = await context.Productos.
                    //Include(x => x.ProductosPaquete).
                    AsNoTracking().
                    Where(x => x.NegocioId == negocioId).
                    OrderBy(x => x.Descripcion).
                    ToListAsync().
                    ConfigureAwait(false);

                respuesta.Datos = productos;
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al consultar los productos {ex.InnerException.Message}";
            }

            return respuesta;
        }

        [HttpGet("GetProductosDTOs/{negocioId}")]
        public async Task<Respuesta<List<ProductoDTO>>> GetProductosDTOs(long negocioId)
        {
            var respuesta = new Respuesta<List<ProductoDTO>>
            {
                Estado = EstadosDeRespuesta.Correcto,
                Mensaje = "Operacion exitosa"
            };

            try
            {
                var productosDTO = await context.Productos.
                    Where(x => x.NegocioId == negocioId).
                    Select(a => new ProductoDTO
                    {
                        Id = a.Id,
                        Descripcion = a.Descripcion,
                        PrecioVenta = a.PrecioVenta,
                        CantidadInventario = a.CantidadInventario,
                        CodigoBarras = a.CodigoBarras
                    }).
                    AsNoTracking().
                    ToListAsync();

                respuesta.Datos = productosDTO;
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al consultar el producto {ex.InnerException.Message}";
            }

            return respuesta;
        }

        [HttpPost]
        [Route("{action}")]
        public async Task<Respuesta<long>> CrearProducto([FromBody] Producto producto)
        {
            var respuesta = new Respuesta<long>
            {
                Estado = EstadosDeRespuesta.Correcto,
                Mensaje = "Operacion exitosa"
            };

            try
            {
                context.Add(producto);
                await context.SaveChangesAsync();

                respuesta.Datos = producto.Id;

                var objetoSerializado = JsonSerializer.Serialize(producto);
                await _logService.LogAsync("Guardar Producto", $"Nuevo producto: {objetoSerializado}", producto.NegocioId);
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al guardar el producto: {producto.Descripcion}";
            }
            return respuesta;
        }

        [HttpPost]
        [Route("{action}")]
        public async Task<Respuesta<long>> EditarProducto([FromBody] Producto producto)
        {
            var respuesta = new Respuesta<long>
            {
                Estado = EstadosDeRespuesta.Correcto,
                Mensaje = "Operacion exitosa"
            };

            try
            {
                var productoAnterior = await context.Productos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == producto.Id);
                var objetoSerializadoAnterior = JsonSerializer.Serialize(productoAnterior);

                //context.Attach(producto).State = EntityState.Modified;
                context.Update(producto);
                await context.SaveChangesAsync();

                respuesta.Datos = producto.Id;

                var objetoSerializado = JsonSerializer.Serialize(producto);
                await _logService.LogAsync("Editar Producto", $"Producto editado de {objetoSerializadoAnterior} a {objetoSerializado}.", producto.NegocioId);
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al actualizar el producto {producto.Descripcion}";
            }

            return respuesta;
        }

        [HttpPost]
        [Route("{action}")]
        public async Task<Respuesta<long>> ActualizarInventario([FromBody] List<ProductoDTO> Productos)
        {
            var respuesta = new Respuesta<long>
            {
                Estado = EstadosDeRespuesta.Correcto,
                Mensaje = "Operacion exitosa"
            };
            var transaction = context.Database.BeginTransaction();

            try
            {
                foreach (var value in Productos)
                {
                    var producto = context.Productos.FirstOrDefault(p => p.Id == value.Id);
                    producto.CantidadInventario += value.CantidadSumadaInventario;
                    await context.SaveChangesAsync();
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al actualizar el inventario";
            }

            return respuesta;
        }

        [HttpPost]
        [Route("{action}")]
        public async Task<Respuesta<long>> EliminarProducto([FromBody] Producto producto)
        {
            var respuesta = new Respuesta<long>
            {
                Estado = EstadosDeRespuesta.Correcto,
                Mensaje = "Operacion exitosa"
            };
            var transaction = context.Database.BeginTransaction();

            try
            {
                context.Productos.Remove(producto);

                await context.SaveChangesAsync();

                transaction.Commit();

                var objetoSerializado = JsonSerializer.Serialize(producto);
                await _logService.LogAsync("Eliminar Producto", $"Producto eliminado {objetoSerializado}.", producto.NegocioId);
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al guardar al eliminar el producto {ex.InnerException.Message}";
            }

            return respuesta;
        }

        [HttpPost]
        [Route("{action}")]
        public async Task<Respuesta<bool>> EliminarProductoPaquete([FromBody] ProductoPaquete producto)
        {
            var respuesta = new Respuesta<bool>
            {
                Estado = EstadosDeRespuesta.Correcto,
                Mensaje = "Operacion exitosa"
            };
            var transaction = context.Database.BeginTransaction();

            try
            {
                context.ProductoPaquetes.Remove(producto);

                await context.SaveChangesAsync();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al guardar al eliminar el producto {ex.InnerException.Message}";
            }

            return respuesta;
        }
    }
}
