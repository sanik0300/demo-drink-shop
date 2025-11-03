using DemoDrinkShop.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DemoDrinkShop.Views.Shared.Components
{
    public class CartSummaryViewComponent : ViewComponent
	{
		private Cart cart;
		public CartSummaryViewComponent(Cart cartService)
		{
			cart = cartService;
		}
		public IViewComponentResult Invoke()
		{
			return View(cart);
		}
	}
}
