using System.ComponentModel.DataAnnotations;

namespace DemoDrinkShop.Presentation.ViewModels
{
    public class UserViewModel
    {
        [EmailAddress]
        public string? Email { get; set; }

        public string? Phone { get; set; }

        [Required]
        [MinLength(6)]
        [UIHint("password")]
        public string? Password { get; set; }

        public bool? VerifyByEmail { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }


        public string ReturnUrl { get; set; } = "/";
    }
}

