﻿using DemoDrinkShop.Infrastructure;

namespace DemoDrinkShop.Models
{
	public class SessionCart : Cart
	{
		public static Cart GetCart(IServiceProvider services)
		{
			ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
			SessionCart cart = session?.GetJson<SessionCart>("Cart") ?? new SessionCart();
			cart.Session = session;
			return cart;
		}

		[Newtonsoft.Json.JsonIgnore]
		public ISession Session { get; set; }

		public override void AddItem(Product product, int quantity)
		{
			base.AddItem(product, quantity);
			Session.SetJson("Cart", this);
		}

		public override void RemoveLine(Product product)
		{
			base.RemoveLine(product);
			Session.SetJson("Cart", this);
		}

		public override void Clear()
		{
			base.Clear();
			Session.Remove("Cart");
		}
	}
}
