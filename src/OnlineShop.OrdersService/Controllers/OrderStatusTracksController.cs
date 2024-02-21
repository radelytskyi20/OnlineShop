using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Library.Common.Interfaces;
using OnlineShop.Library.Common.Repos;
using OnlineShop.Library.OrdersService.Models;

namespace OnlineShop.OrdersService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class OrderStatusTracksController : RepoControllerBase<OrderStatusTrack>
    {
        public OrderStatusTracksController(IRepo<OrderStatusTrack> entitiesRepo) : base(entitiesRepo) { }

        protected override void UpdateProperties(OrderStatusTrack source, OrderStatusTrack destination)
        {
            destination.Assigned = source.Assigned;
        }
    }
}
