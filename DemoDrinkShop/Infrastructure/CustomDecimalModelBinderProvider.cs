using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DemoDrinkShop.Infrastructure
{
	public class CustomDecimalModelBinderProvider : IModelBinderProvider
	{
		public IModelBinder? GetBinder(ModelBinderProviderContext context)
		{
			if(context.Metadata.ModelType != typeof(decimal)) { return null; }

			return new CustomDecimalModelBinder();
		}
	}
}
