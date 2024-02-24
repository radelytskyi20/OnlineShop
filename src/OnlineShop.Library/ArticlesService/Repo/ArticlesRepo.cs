using Microsoft.EntityFrameworkCore;
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

        public override async Task<IEnumerable<Article>> GetAllAsync() =>
            await Table.Include(nameof(Article.PriceLists)).ToListAsync();

        public override async Task<Article> GetOneAsync(Guid id) =>
            await Task.Run(() => Table.Include(nameof(Article.PriceLists))
                .FirstOrDefault(entity => entity.Id == id));
    }
}
