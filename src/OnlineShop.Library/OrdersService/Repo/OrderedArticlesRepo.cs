using OnlineShop.Library.ArticlesService.Models;
using OnlineShop.Library.Common.Repos;
using OnlineShop.Library.Data;

namespace OnlineShop.Library.OrdersService.Repo
{
    public class OrderedArticlesRepo : BaseRepo<OrderedArticle>
    {
        public OrderedArticlesRepo(OrdersDbContext context) : base(context) { }
    }
}
