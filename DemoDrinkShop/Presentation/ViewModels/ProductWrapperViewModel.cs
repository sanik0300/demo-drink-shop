using DemoDrinkShop.Domain.Entities;

namespace DemoDrinkShop.Presentation.ViewModels
{
    public class ProductWrapperViewModel
    {
        public Product Product { get; set; }
        public string? ImageSrc { get; set; }
        public bool? ImageExists { get; set; }
    }
}
