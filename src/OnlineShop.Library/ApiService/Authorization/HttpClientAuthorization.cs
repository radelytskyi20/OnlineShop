using IdentityModel.Client;
using Microsoft.Extensions.Options;
using OnlineShop.Library.Clients;
using OnlineShop.Library.Clients.IdentityServer;
using OnlineShop.Library.Options;

namespace OnlineShop.Library.ApiService.Authorization
{
    /// <summary>
    /// Provides functionality for authorizing HTTP clients using an identity server
    /// </summary>
    public class HttpClientAuthorization : IClientAuthorization
    {
        private readonly IIdentityServerClient _identityClient;
        private readonly IdentityServerApiOptions _identityServerApiOptions;

        public HttpClientAuthorization(IIdentityServerClient identityClient, IOptions<IdentityServerApiOptions> options)
        {
            _identityClient = identityClient;
            _identityServerApiOptions = options.Value;
        }

        public async Task Authorize(IHttpClientContainer clientContainer)
        {
            if (clientContainer == null)
                return;

            var token = await _identityClient.GetApiToken(_identityServerApiOptions);
            clientContainer.HttpClient.SetBearerToken(token.AccessToken);
        }
    }

}
