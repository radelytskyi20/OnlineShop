using OnlineShop.Ui.Abstractions.Interfaces;
using OnlineShop.Ui.Models.Articles;

namespace OnlineShop.Ui.Abstractions
{
    public class ArticlesProvider : IArticlesProvider
    {
        private readonly HttpClient _client;

        public ArticlesProvider(HttpClient client) { _client = client; }

        public async Task<IEnumerable<Article>> GetArticles()
        {
            var articles = await _client.GetFromJsonAsync<List<Article>>("articles/all") ?? new();
            var priceLists = await _client.GetFromJsonAsync<List<PriceList>>("pricelists/all") ?? new();

            articles.ForEach(article =>
            {
                var price = priceLists.FirstOrDefault(price => price.ArticleId == article.Id)?.Price ?? 0M;
                article.Price = price;
            });

            return articles;
        }

        public async Task<Article> GetArticle(Guid articleId)
        {
            var article = await _client.GetFromJsonAsync<Article>($"articles?id={articleId}") ?? new();
            var priceLists = await _client.GetFromJsonAsync<List<PriceList>>("pricelists/all") ?? new();

            article.Price = priceLists.FirstOrDefault(price => price.ArticleId == article.Id)?.Price ?? 0M;
            return article;
        }
    }
}
