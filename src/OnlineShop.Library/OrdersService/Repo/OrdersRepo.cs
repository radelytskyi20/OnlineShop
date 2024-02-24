using Microsoft.EntityFrameworkCore;
using OnlineShop.Library.Common.Repos;
using OnlineShop.Library.Data;
using OnlineShop.Library.OrdersService.Models;

namespace OnlineShop.Library.OrdersService.Repo
{
    public class OrdersRepo : BaseRepo<Order>
    {
        public OrdersRepo(OrdersDbContext context) : base(context) { Table = Context.Orders; }

        public override async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await Table
                .Include(nameof(Order.Articles))
                .Include(o => o.OrderStatusTracks)
                    .ThenInclude(ost => ost.OrderStatus)
                .ToListAsync();
        }
        public override async Task<Order> GetOneAsync(Guid id)
        {
            return await Table.Include(nameof(Order.Articles))
                .Include(o => o.OrderStatusTracks)
                    .ThenInclude(ost => ost.OrderStatus)
                .FirstOrDefaultAsync(entity => entity.Id == id);
        }
    }
}
