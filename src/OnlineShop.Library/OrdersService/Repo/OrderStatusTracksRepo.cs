using Microsoft.EntityFrameworkCore;
using OnlineShop.Library.Common.Repos;
using OnlineShop.Library.Data;
using OnlineShop.Library.OrdersService.Models;

namespace OnlineShop.Library.OrdersService.Repo
{
    public class OrderStatusTracksRepo : BaseRepo<OrderStatusTrack>
    {
        public OrderStatusTracksRepo(OrdersDbContext context) : base(context) { }
        public override async Task<OrderStatusTrack> GetOneAsync(Guid id) => await Table.Include(nameof(OrderStatusTrack.OrderStatus)).FirstOrDefaultAsync(entity => entity.Id == id);
        public override async Task<IEnumerable<OrderStatusTrack>> GetAllAsync() => await Table.Include(nameof(OrderStatusTrack.OrderStatus)).ToListAsync();
    }
}
