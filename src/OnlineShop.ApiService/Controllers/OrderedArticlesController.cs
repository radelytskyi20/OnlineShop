using Microsoft.AspNetCore.Mvc;
using OnlineShop.ApiService.Authorization;
using OnlineShop.Library.ArticlesService.Models;
using OnlineShop.Library.Clients;
using OnlineShop.Library.Constants;

namespace OnlineShop.ApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderedArticlesController : ControllerWithClientAuthorization<IRepoClient<OrderedArticle>>
    {
        public OrderedArticlesController(IRepoClient<OrderedArticle> client, IClientAuthorization clientAuthorization) : base(client, clientAuthorization)
        { }

        [HttpPost(RepoActions.Add)]
        public async Task<IActionResult> Add([FromBody] OrderedArticle orderedArticle)
        {
            var response = await Client.Add(orderedArticle);
            return Ok(response.Payload);
        }

        [HttpPost(RepoActions.AddRange)]
        public async Task<IActionResult> AddRange([FromBody] IEnumerable<OrderedArticle> orderedArticles)
        {
            var response = await Client.AddRange(orderedArticles);
            return Ok(response.Payload);
        }

        [HttpGet]
        public async Task<IActionResult> GetOne([FromQuery] Guid id)
        {
            var response = await Client.GetOne(id);
            return Ok(response.Payload);
        }

        [HttpGet(RepoActions.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var response = await Client.GetAll();
            return Ok(response.Payload);
        }

        [HttpPost(RepoActions.Remove)]
        public virtual async Task<IActionResult> Remove([FromBody] Guid id)
        {
            await Client.Remove(id);
            return NoContent();
        }

        [HttpPost(RepoActions.AddRange)]
        public virtual async Task<IActionResult> RemoveRange([FromBody] IEnumerable<Guid> ids)
        {
            await Client.RemoveRange(ids);
            return NoContent();
        }

        [HttpPost(RepoActions.Update)]
        public virtual async Task<IActionResult> Update([FromBody] OrderedArticle orderedArticle)
        {
            var response = await Client.Update(orderedArticle);
            return Ok(response.Payload);
        }
    }
}
