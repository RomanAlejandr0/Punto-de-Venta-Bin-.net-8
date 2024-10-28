using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using PuntoVentaBin.Client.Seguridad;

public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly ILoginService _loginService;

    public AuthorizationMessageHandler(ILoginService loginService)
    {
        _loginService = loginService;
    }

    //protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    //{
    //    var token = await _loginService.GetTokenAsync();

    //    if (!string.IsNullOrEmpty(token))
    //    {
    //        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    //    }

    //    return await base.SendAsync(request, cancellationToken);
    //}
}
