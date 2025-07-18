﻿using System.ComponentModel.DataAnnotations;

namespace DemoDrinkShop.Models.ViewModels
{
	public class LoginModel
	{
		[Required]
		public string? Name { get; set; }

		[Required]
		[UIHint("password")]
		public string? Password { get; set; }

		public string ReturnUrl { get; set; } = "/";
	}
}

