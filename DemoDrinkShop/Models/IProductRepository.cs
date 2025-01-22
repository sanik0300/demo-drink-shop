namespace DemoDrinkShop.Models
{
    public interface IProductRepository
    {
        IEnumerable<Product> Products { get; }
        void SaveProduct(Product product);
		Product DeleteProduct(int productId);
	}
}
