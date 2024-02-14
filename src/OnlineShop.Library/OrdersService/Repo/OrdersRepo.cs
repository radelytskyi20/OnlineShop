using Microsoft.EntityFrameworkCore;
using OnlineShop.Library.Common.Repos;
using OnlineShop.Library.Data;
using OnlineShop.Library.OrdersService.Models;

namespace OnlineShop.Library.OrdersService.Repo
{
    public class OrdersRepo : BaseRepo<Order>
    {
        public OrdersRepo(OrdersDbContext context) : base(context) { Table = Context.Orders; }

        public override async Task<IEnumerable<Order>> GetAllAsync() => await Table.Include(nameof(Order.Articles)).ToListAsync();
        public override async Task<Order> GetOneAsync(Guid id) => await Task.Run(() => Table.Include(nameof(Order.Articles)).FirstOrDefault(entity => entity.Id == id));
    }
}
