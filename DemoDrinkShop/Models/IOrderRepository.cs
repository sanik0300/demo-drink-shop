namespace DemoDrinkShop.Models
{
	public interface IOrderRepository
	{
		IEnumerable<Order> Orders { get; }
		void SaveOrder(Order order);
	}
}
