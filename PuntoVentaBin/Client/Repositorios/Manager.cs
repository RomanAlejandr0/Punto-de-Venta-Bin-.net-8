using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PuntoVentaBin.Shared;

namespace PuntoVentaBin.Client.Repositorios
{
    public class Manager : IManager
    {
        private HttpClient client;

        public Manager(HttpClient client)
        {
            this.client = client;
        }

        public async Task<Respuesta<T>> Get<T>(string url)
        {
            var response = await client.GetAsync(url);
            var respuesta = JsonSerializer.Deserialize<Respuesta<T>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return respuesta;
        }

        public async Task<Respuesta<R>> Post<T, R>(string url, T enviar)
        {
            var respuesta = new Respuesta<R>();
            var enviarJSON = string.Empty;
            try
            {
                enviarJSON = JsonSerializer.Serialize(enviar);
                var content = new StringContent(enviarJSON, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                var cadenaRespuesta = await response.Content.ReadAsStringAsync();
                respuesta = JsonSerializer.Deserialize<Respuesta<R>>(cadenaRespuesta, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"RESPUESTA: {respuesta}" + ex.ToString();
            }

            return respuesta;
        }

        public async Task<Respuesta<string>> PostString<T>(string url, T enviar)
        {
            var respuesta = new Respuesta<string>();
            var enviarJSON = string.Empty;
            try
            {
                enviarJSON = JsonSerializer.Serialize(enviar);
                var content = new StringContent(enviarJSON, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                respuesta.Datos = await response.Content.ReadAsStringAsync();
                var deseralizado = JsonSerializer.Deserialize<Respuesta<string>>(respuesta.Datos, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (deseralizado.Estado == EstadosDeRespuesta.Correcto)
                {
                    respuesta.Datos = deseralizado.Datos;
                }

            }
            catch (Exception ex)
            {
                return new Respuesta<string> { Mensaje = ex.Message, Datos = ex.Message };
            }

            return respuesta;
        }
    }
}
