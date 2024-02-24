using OnlineShop.Library.ArticlesService.Models;
using OnlineShop.Library.Common.Repos;
using OnlineShop.Library.Data;

namespace OnlineShop.Library.ArticlesService.Repo
{
    public class PriceListsRepo : BaseRepo<PriceList>
    {
        public PriceListsRepo(OrdersDbContext context) : base (context)
        {
            Table = Context.PriceLists;
        }
    }
}
