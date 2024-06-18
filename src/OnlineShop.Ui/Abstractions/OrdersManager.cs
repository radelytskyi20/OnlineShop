using Microsoft.JSInterop;
using Newtonsoft.Json;
using OnlineShop.Ui.Abstractions.Interfaces;
using OnlineShop.Ui.Models.Articles;
using OnlineShop.Ui.Models.Orders;
using System.Text;

namespace OnlineShop.Ui.Abstractions
{
    public class OrdersManager : IOrdersManager
    {
        private readonly HttpClient _client;
        private readonly ILoginStatusManager _loginStatusManager;
        private readonly IJSRuntime _jSRuntime;

        public OrdersManager(HttpClient client, ILoginStatusManager loginStatusManager, IJSRuntime jSRuntime)
        {
            _client = client;
            _loginStatusManager = loginStatusManager;
            _jSRuntime = jSRuntime;
        }

        public async Task SubmitOrder(Guid addressId, Dictionary<Article, int> cartItems)
        {
            var priceLists = await _client.GetFromJsonAsync<List<PriceList>>("pricelists/all") ?? new();

            var orderedArticles = cartItems.Select(kvp =>
            {
                var priceList = priceLists.FirstOrDefault(price => price.ArticleId == kvp.Key.Id);
                var orderedArticle = new OrderedArticle
                {
                    Name = kvp.Key.Name,
                    Description = kvp.Key.Description,
                    Price = kvp.Key.Price,
                    Quantity = kvp.Value,
                    PriceListName = priceList?.Name ?? "Default",
                    ValidFrom = priceList?.ValidFrom ?? DateTime.MinValue,
                    ValidTo = priceList?.ValidTo ?? DateTime.MinValue
                };

                return orderedArticle;
            })
            .ToList();

            var order = new Order
            {
                UserId = _loginStatusManager.LoginStatus.User.Id,
                AddressId = addressId,
                Articles = orderedArticles
            };

            var jsonContent = JsonConvert.SerializeObject(order);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var requestResult = await _client.PostAsync("orders/add", httpContent);

            if (requestResult.IsSuccessStatusCode)
            {
                cartItems.Clear();
                CartStatusHasChanged?.Invoke(this, EventArgs.Empty);
                await _jSRuntime.InvokeVoidAsync("alert", "Your order has been submitted. Thank you!");
                return;
            }

            await _jSRuntime.InvokeVoidAsync("alert", $"Failed to send a request. Response: {requestResult.StatusCode}");
        }

        public async Task<IEnumerable<OrderView>> GetAll()
        {
            var orders = await _client.GetFromJsonAsync<List<Order>>("orders/all") ?? new();
            var userOrders = orders.Where(o => o.UserId == _loginStatusManager.LoginStatus.User.Id);

            var orderViews = userOrders.Select(order =>
            {
                var orderView = new OrderView
                {
                    Id = order.Id,
                    User = _loginStatusManager.LoginStatus.User,
                    Address = _loginStatusManager.LoginStatus.User.DeliveryAddress,
                    Created = order.Created,
                    Modified = order.Modified,
                    Articles = order.Articles
                };

                return orderView;
            })
            .ToList();

            return orderViews;
        }


        public event EventHandler? CartStatusHasChanged;
    }
}
