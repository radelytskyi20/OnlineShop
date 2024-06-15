using Microsoft.JSInterop;
using OnlineShop.Ui.Abstractions.Interfaces;
using OnlineShop.Ui.Models.Articles;

namespace OnlineShop.Ui.States
{
    public class CartState
    {
        private readonly IArticlesProvider _articlesProvider;
        private readonly IJSRuntime _jSRuntime;

        public Dictionary<Article, int> Items { get; set; } = new();

        public CartState(IArticlesProvider articlesProvider, IJSRuntime jSRuntime)
        {
            _articlesProvider = articlesProvider;
            _jSRuntime = jSRuntime;
        }

        public async Task AddArticleToCartAsync(Guid articleId, int quantity)
        {
            if (!Items.Any(kvp => kvp.Key.Id == articleId))
            {
                var article = await _articlesProvider.GetArticle(articleId);
                if (article == null || quantity <= 0) return;

                Items.Add(article, quantity);
                await _jSRuntime.InvokeVoidAsync("alert", $"{article.Name} was added to cart. Click Checkout button to see items in the cart.");
            }
        }
    }
}
