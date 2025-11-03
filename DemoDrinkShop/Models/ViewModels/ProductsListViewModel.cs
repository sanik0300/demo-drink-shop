namespace DemoDrinkShop.Models.ViewModels
{
    public class ProductsListViewModel
    {
        public IList<ProductWrapperViewModel>? Products { get; set; } 
        public PagingInfo? PagingInfo { get; set; }
		public byte? CurrentCategory { get; set; }
	}
}
