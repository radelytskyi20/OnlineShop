using OnlineShop.Ui.Abstractions.Interfaces;
using OnlineShop.Ui.Models.Articles;

namespace OnlineShop.Ui.States
{
    public class CartState
    {
        private readonly IArticlesProvider _articlesProvider;

        public Dictionary<Article, int> Items { get; set; } = new();

        public CartState(IArticlesProvider articlesProvider)
        {
            _articlesProvider = articlesProvider;
        }

        public async Task<bool> AddArticleToCartAsync(Guid articleId, int quantity)
        {
            var article = await _articlesProvider.GetArticle(articleId);
            if (article == null || quantity <= 0) return false;

            if (!Items.Any(kvp => kvp.Key.Id == articleId))
            {
                Items.Add(article, quantity);
                
            }
            else
            {
                var item = Items.First(kvp => kvp.Key.Id == articleId);
                Items[item.Key] = item.Value + quantity;
            }

            return true;
        }
    }
}
