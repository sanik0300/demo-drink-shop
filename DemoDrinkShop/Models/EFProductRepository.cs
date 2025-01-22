namespace DemoDrinkShop.Models
{
    public class EFProductRepository : IProductRepository
    {
        private ApplicationDbContext context;
        public IEnumerable<Product> Products => context.Products;

        public EFProductRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public void SaveProduct(Product product)
        {
            if (product.ProductID == 0)
            {
                context.Products.Add(product);
            }
            else {
                Product dbEntry = context.Products.FirstOrDefault(p => p.ProductID == product.ProductID);
                if (dbEntry != null)
                {
                    dbEntry.Name = product.Name;
                    dbEntry.Description = product.Description;
                    dbEntry.Price = product.Price;
                    dbEntry.Category = product.Category;
                    dbEntry.ImageURL = product.ImageURL;
                }
            }
            context.SaveChanges();
        }

		public Product DeleteProduct(int productId)
		{
			Product dbEntry = context.Products.FirstOrDefault(p => p.ProductID == productId);
			if (dbEntry != null)
			{
				context.Products.Remove(dbEntry);
				context.SaveChanges();
			}
			return dbEntry;
		}
	}
}

