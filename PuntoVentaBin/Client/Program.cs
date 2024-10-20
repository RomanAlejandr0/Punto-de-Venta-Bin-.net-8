using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PuntoVentaBin.Client;
using PuntoVentaBin.Client.Helpers;
using PuntoVentaBin.Client.Repositorios;
using PuntoVentaBin.Client.Seguridad;
using PuntoVentaBIN.Client.Repositorios;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<IManager, Manager>();
builder.Services.AddScoped<IMostrarMensajes, MostrarMensajes>();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<ProveedorAutenticacion>();

builder.Services.AddScoped<AuthenticationStateProvider, ProveedorAutenticacion>(
    prov => prov.GetRequiredService<ProveedorAutenticacion>());

builder.Services.AddScoped<ILoginService, ProveedorAutenticacion>(
    prov => prov.GetRequiredService<ProveedorAutenticacion>());

await builder.Build().RunAsync();
