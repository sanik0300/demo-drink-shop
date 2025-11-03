using DemoDrinkShop.Infrastructure;
using DemoDrinkShop.Models;
using DemoDrinkShop.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DemoDrinkShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository repository;
        private readonly IImageStorageService _imagesService;

        private async Task<ProductWrapperViewModel> GetMiniProductWrapper(Product product)
        {
            var wrapper = new ProductWrapperViewModel() { Product = product };
            if (product.ImageURL != null)
            {
                bool exists = await _imagesService.ImageExists(product.ImageURL);
                wrapper.ImageExists = exists;
                if(exists)
                {
                    wrapper.ImageSrc = _imagesService.GetEndURL(product.ImageURL);
                }
            }
            return wrapper;
        }

        public int PageSize { get; set; }


        public ProductController(IProductRepository repo, IImageStorageService storageService)
        {
            repository = repo;
            _imagesService = storageService;

            PageSize = 4;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List(string? category, int page = 1)
        {  
            byte? catByte = null;
            int index = Array.IndexOf(Product.CategoryNames.ToArray(), category);
            if (category!=null && index>=0)
            {
                catByte = (byte)index;
            }

            return View(new ProductsListViewModel()
            {
                Products = repository.Products
                                    .Where(p => category == null || (byte)p.Category == catByte)
                                    .OrderBy(p => p.ProductID)
                                    .Skip((page - 1) * PageSize)
                                    .Take(PageSize)
                                    .Select(pr => GetMiniProductWrapper(pr).Result)
                                    .ToArray(),
                PagingInfo = new PagingInfo()
                {
                    CurrentPage = page,
                    CurrentCategory = catByte,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null ? repository.Products.Count() :
                                               repository.Products.Where(p => (byte)p.Category == catByte).Count()
                },
                CurrentCategory = catByte
            });
        }
    }
}
