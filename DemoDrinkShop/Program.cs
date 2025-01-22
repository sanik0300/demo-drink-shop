using DemoDrinkShop.Infrastructure;
using DemoDrinkShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace DemoDrinkShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("uk-UA");

            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.UseDefaultServiceProvider(options => options.ValidateScopes = false);

            ConfigurationManager configuration = builder.Configuration;
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration["Data:DemoDrinkShopProducts:ConnectionStrings"]);
            });
            builder.Services.AddTransient<IProductRepository, EFProductRepository>();
			builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
			builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			builder.Services.AddTransient<IOrderRepository, EFOrderRepository>();
			builder.Services.AddDbContext<AppIdentityDbContext>(options =>
			{
				options.UseSqlServer(configuration["Data:DemoDrinkShopIdentity:ConnectionString"]);
			});
			builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();


			builder.Services.AddMvc(options => 
			{
				options.EnableEndpointRouting = false;
				options.ModelBinderProviders.Insert(0, new CustomDecimalModelBinderProvider());
			});
			builder.Services.AddMemoryCache();
			builder.Services.AddSession();
			var app = builder.Build();

            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
			app.UseStaticFiles(new StaticFileOptions() { ServeUnknownFileTypes = true });

            app.MapControllerRoute("pagination", "Products/Page{page}", new { Controller = "Product", Action = "List" });

			app.UseAuthentication();
			app.UseAuthorization();
			app.UseSession();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
				name: null,
				template: "{category}/Page{page:int}",
				defaults: new
				{
					controller = "Product",
					action = "List"
				});

				routes.MapRoute(
				name: null,
				template: "Page{page:int}",
				defaults: new
				{
					controller = "Product",
					action = "List",
					page = 1
				});

				routes.MapRoute(
				name: null,
				template: "{category}",
				defaults: new
				{
					controller = "Product",
					action = "List",
					page = 1
				});

				routes.MapRoute(
				name: null,
				template: "",
				defaults: new
				{
					controller = "Product",
					action = "List",
					page = 1
				});

				routes.MapRoute(name: null, template: "{controller}/{action}/{id?}");
			});


			SeedData.EnsurePopulated(app);
			IdentitySeedData.EnsurePopulated(app);

			app.Run();
        }
    }
}