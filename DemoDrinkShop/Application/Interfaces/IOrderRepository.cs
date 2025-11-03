using DemoDrinkShop.Domain.Entities;

namespace DemoDrinkShop.Application.Interfaces
{
    public interface IOrderRepository
    {
        IEnumerable<Order> Orders { get; }
        void SaveOrder(Order order);
    }
}
