using Microsoft.Extensions.Options;
using OnlineShop.Library.ArticlesService.Models;
using OnlineShop.Library.Constants;
using OnlineShop.Library.Options;

namespace OnlineShop.Library.Clients.ArticlesService
{
    public class ArticlesClient : RepoClientBase<Article>
    {
        public ArticlesClient(HttpClient client, IOptions<ServiceAdressOptions> options) : base(client, options) { }

        protected override void InitializeClient(IOptions<ServiceAdressOptions> options) => HttpClient.BaseAddress = new Uri(options.Value.ArticlesService);
        protected override void SetControllerName() => ControllerName = ControllerNames.Articles;
    }
}
