using OnlineShop.Ui.Models.Articles;

namespace OnlineShop.Ui.Abstractions.Interfaces
{
    public interface IArticlesProvider
    {
        Task<IEnumerable<Article>> GetArticles();
    }
}
