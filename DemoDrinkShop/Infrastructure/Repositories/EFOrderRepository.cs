using DemoDrinkShop.Application.Interfaces;
using DemoDrinkShop.Domain.Entities;
using DemoDrinkShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DemoDrinkShop.Infrastructure.Repositories
{
    public class EFOrderRepository : IOrderRepository
    {
        private ApplicationDbContext context;

        public EFOrderRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<Order> Orders => context.Orders.Include(o => o.Lines).ThenInclude(l => l.Product);

        public void SaveOrder(Order order)
        {
            context.AttachRange(order.Lines.Select(l => l.Product));
            if (order.OrderID == 0)
            {
                context.Orders.Add(order);
            }
            context.SaveChanges();
        }
    }
}
