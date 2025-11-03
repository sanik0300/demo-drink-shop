using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DemoDrinkShop.Infrastructure
{
	public class CustomDecimalModelBinder : IModelBinder
	{
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			ValueProviderResult val = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

			string fv = val.FirstValue;

			decimal modelNumber=0;
			bool ok = fv!=null && decimal.TryParse(fv.Replace('.', ','), out modelNumber);

			bindingContext.Result = ok ? ModelBindingResult.Success(modelNumber) : ModelBindingResult.Failed();
			return Task.CompletedTask;
		}
	}
}
