using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Web;

namespace DemoDrinkShop.Presentation.Views.Shared.Components
{
    public class LoginStateViewComponent : ViewComponent
    {
        private string GetLoginText()
        {
            ClaimsPrincipal crtUser = this.HttpContext.User;
            if (crtUser.Identity == null || !crtUser.Identity.IsAuthenticated)
            {
                return "Not logged in";
            }
            string nameFromClaim = crtUser.FindFirst(ClaimTypes.Name)!.Value;
            return $"Logged in as {nameFromClaim}";
        }
        public IViewComponentResult Invoke()
        {
            HtmlString hetemel = new HtmlString($"<p>{GetLoginText()}</p>");
            return new HtmlContentViewComponentResult(hetemel);
        }
    }
}
