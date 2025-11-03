using DemoDrinkShop.Domain.Entities;

namespace DemoDrinkShop.Application.Interfaces
{
    public interface IProductRepository
    {
        IEnumerable<Product> Products { get; }
        void SaveProduct(Product product);
        Product DeleteProduct(int productId);
    }
}
