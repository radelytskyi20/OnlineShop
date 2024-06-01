using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.ApiService.Authorization;
using OnlineShop.Library.ArticlesService.Models;
using OnlineShop.Library.Clients;
using OnlineShop.Library.Constants;

namespace OnlineShop.ApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class PriceListsController : ControllerWithClientAuthorization<IRepoClient<PriceList>>
    {
        public PriceListsController(IRepoClient<PriceList> client, IClientAuthorization clientAuthorization) : base(client, clientAuthorization)
        { }

        [HttpPost(RepoActions.Add)]
        public async Task<IActionResult> Add([FromBody] PriceList priceList)
        {
            var response = await Client.Add(priceList);
            return Ok(response.Payload);
        }

        [HttpPost(RepoActions.AddRange)]
        public async Task<IActionResult> AddRange([FromBody] IEnumerable<PriceList> priceLists)
        {
            var response = await Client.AddRange(priceLists);
            return Ok(response.Payload);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetOne([FromQuery] Guid id)
        {
            var response = await Client.GetOne(id);
            return Ok(response.Payload);
        }

        [HttpGet(RepoActions.GetAll)]
        [AllowAnonymous]
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

        [HttpPost(RepoActions.RemoveRange)]
        public virtual async Task<IActionResult> RemoveRange([FromBody] IEnumerable<Guid> ids)
        {
            await Client.RemoveRange(ids);
            return NoContent();
        }

        [HttpPost(RepoActions.Update)]
        public virtual async Task<IActionResult> Update([FromBody] PriceList priceList)
        {
            var response = await Client.Update(priceList);
            return Ok(response.Payload);
        }
    }
}
