using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.ApiService.Authorization;
using OnlineShop.Library.Clients;
using OnlineShop.Library.Constants;
using OnlineShop.Library.OrdersService.Models;

namespace OnlineShop.ApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class OrderStatusTracksController : ControllerWithClientAuthorization<IRepoClient<OrderStatusTrack>>
    {
        public OrderStatusTracksController(IRepoClient<OrderStatusTrack> client, IClientAuthorization clientAuthorization) : base(client, clientAuthorization)
        { }

        [HttpPost(RepoActions.Add)]
        public async Task<IActionResult> Add([FromBody] OrderStatusTrack orderStatusTrack)
        {
            var response = await Client.Add(orderStatusTrack);
            return Ok(response.Payload);
        }

        [HttpPost(RepoActions.AddRange)]
        public async Task<IActionResult> AddRange([FromBody] IEnumerable<OrderStatusTrack> orderStatusTracks)
        {
            var response = await Client.AddRange(orderStatusTracks);
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
            var response = await Client.Remove(id);
            return Ok(response.Payload);
        }

        [HttpPost(RepoActions.RemoveRange)]
        public virtual async Task<IActionResult> RemoveRange([FromBody] IEnumerable<Guid> ids)
        {
            var response = await Client.RemoveRange(ids);
            return Ok(response.Payload);
        }

        [HttpPost(RepoActions.Update)]
        public virtual async Task<IActionResult> Update([FromBody] OrderStatusTrack orderStatusTrack)
        {
            var response = await Client.Update(orderStatusTrack);
            return Ok(response.Payload);
        }
    }
}
