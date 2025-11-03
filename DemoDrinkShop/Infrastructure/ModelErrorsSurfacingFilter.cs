using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace DemoDrinkShop.Infrastructure
{
    public class ModelErrorsSurfacingFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.ModelState.ErrorCount == 0)
            {
                base.OnActionExecuted(context);
                return;
            }

            string totalTempMessage = context.ModelState.SelectMany(x => x.Value?.Errors)
                                                        .Where(x => x != null)
                                                        .Select(x => x.ErrorMessage)
                                                        .Aggregate((c, n) => $"{c}\n{n}");
            
            (context.Controller as Controller).TempData["message"] = totalTempMessage;
            base.OnActionExecuted(context);
        }
    }
}
