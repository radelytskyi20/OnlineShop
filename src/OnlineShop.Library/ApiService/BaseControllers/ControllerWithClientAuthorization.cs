using Microsoft.AspNetCore.Mvc;
using OnlineShop.Library.ApiService.Authorization;
using OnlineShop.Library.Clients;

namespace OnlineShop.Library.ApiService.BaseControllers
{
    /// <summary>
    /// Provides a base controller with client authorization.   
    /// All controllers that require a client with authorization should inherit from this class.
    /// This class authorize all requests from clients to services like UserManagementService, OrderService, etc (authorization between services api scope)
    /// </summary>
    /// <typeparam name="TClient">The type of the HTTP client container</typeparam>
    public abstract class ControllerWithClientAuthorization<TClient> : ControllerBase
        where TClient : IHttpClientContainer
    {
        protected readonly TClient Client;

        public ControllerWithClientAuthorization(TClient client, IClientAuthorization clientAuthorization)
        {
            Client = client;
            clientAuthorization.Authorize(Client).Wait();
        }
    }
}
