using OnlineShop.Library.Clients;

namespace OnlineShop.Library.ApiService.Authorization
{
    /// <summary>
    /// Defines contract to authorize requests from API to particular services like UserManagementService, OrderService, etc
    /// </summary>
    public interface IClientAuthorization
    {
        /// <summary>
        /// Authorizes the specified client container. To use client for authorization, it should implement IHttpClientContainer
        /// </summary>
        Task Authorize(IHttpClientContainer clientContainer);
    }

}
