using DemoDrinkShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace DemoDrinkShop.Components
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        private IProductRepository repository;

        public NavigationMenuViewComponent(IProductRepository repo)
        {
            repository = repo;
        }

        public IViewComponentResult Invoke()
        {
            object? routeValue = RouteData?.Values["category"];
            if(routeValue != null)
            {
                int index = Array.IndexOf(Product.CategoryNames.ToArray(), (string)routeValue);
                if(index >= 0)
                {
                    ViewBag.SelectedCategory = index;
                }
            }
            return View(repository.Products.Select(x => x.Category).Distinct().OrderBy(x => x));
        }

    }
}
