using OnlineShop.Ui.Models.Articles;
using OnlineShop.Ui.Models.Orders;

namespace OnlineShop.Ui.Abstractions.Interfaces
{
    public interface IOrdersManager
    {
        Task SubmitOrder(Guid addressId, Dictionary<Article, int> cartItems);
        Task<IEnumerable<OrderView>> GetAll();
        event EventHandler? CartStatusHasChanged;
    }
}
