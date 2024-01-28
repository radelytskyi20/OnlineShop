using OnlineShop.Library.ArticlesService.Models;
using OnlineShop.Library.Common.Repos;
using OnlineShop.Library.Data;

namespace OnlineShop.Library.ArticlesService.Repo
{
    public class ArticlesRepo : BaseRepo<Article>
    {
        public ArticlesRepo(OrdersDbContext context) : base(context)
        {
            Table = Context.Articles;
        }
    }
}
