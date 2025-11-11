using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoDrinkShop.Presentation.ViewModels
{
    public class PasswordChangeViewModel
    {
        [BindProperty(Name = "emailTo")]
        public string Email { get; set; }
        public int Code { get; set; }

        [BindProperty(Name ="password")]
        public string NewPassword { get; set; }
    }
}
