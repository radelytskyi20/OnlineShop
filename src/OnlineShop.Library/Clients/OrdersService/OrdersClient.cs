using Microsoft.Extensions.Options;
using OnlineShop.Library.Constants;
using OnlineShop.Library.Options;
using OnlineShop.Library.OrdersService.Models;

namespace OnlineShop.Library.Clients.OrdersService
{
    public class OrdersClient : RepoClientBase<Order>
    {
        public OrdersClient(HttpClient client, IOptions<ServiceAdressOptions> options) : base(client, options) { }

        protected override void InitializeClient(IOptions<ServiceAdressOptions> options) => HttpClient.BaseAddress = new Uri(options.Value.OrdersService);
        protected override void SetControllerName() => ControllerName = ControllerNames.Orders;
    }
}
