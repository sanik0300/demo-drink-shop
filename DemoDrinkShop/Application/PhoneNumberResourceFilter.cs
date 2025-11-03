using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace DemoDrinkShop.Application
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PhoneNumberResourceFilterAttribute : Attribute, IResourceFilter
    {
        private const string countryCodeKey = "countryCode", restKey = "restOfPhone", phoneKey = "phone";
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            IFormCollection formData = context.HttpContext.Request.Form;

            StringValues code = "", rest = "";
            formData.TryGetValue(countryCodeKey, out code);
            formData.TryGetValue(restKey, out rest);

            string codePart = code.First();
            if (codePart.StartsWith('+'))
            {
                codePart = codePart.Substring(1);
            }

            string fullPhone = codePart + rest.First();

            if (fullPhone == string.Empty || string.IsNullOrWhiteSpace(fullPhone)) { return; }

            var dictionary = formData.Keys.Select(k => new KeyValuePair<string, StringValues>(k, formData[k])).ToDictionary();

            dictionary[phoneKey] = fullPhone;
            dictionary.Remove(countryCodeKey);
            dictionary.Remove(restKey);

            var formCollection = new FormCollection(dictionary);

            context.HttpContext.Request.Form = formCollection;
        }

        public void OnResourceExecuted(ResourceExecutedContext context) { }
    }
}
