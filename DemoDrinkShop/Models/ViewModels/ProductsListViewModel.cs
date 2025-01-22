namespace DemoDrinkShop.Models.ViewModels
{
    public class ProductsListViewModel
    {
        public IList<Product>? Products { get; set; } 
        public PagingInfo? PagingInfo { get; set; }
		public byte? CurrentCategory { get; set; }
	}
}
