using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DemoDrinkShop.Presentation.ModelBinders
{
    public class CustomDecimalModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType != typeof(decimal)) { return null; }

            return new CustomDecimalModelBinder();
        }
    }
}
