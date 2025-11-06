using DemoDrinkShop.Application.Interfaces;
using DemoDrinkShop.Domain;
using DemoDrinkShop.Domain.Entities;
using DemoDrinkShop.Infrastructure;
using DemoDrinkShop.Infrastructure.Identity;
using DemoDrinkShop.Infrastructure.Persistence;
using DemoDrinkShop.Infrastructure.Repositories;
using DemoDrinkShop.Presentation.ModelBinders;
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
                options.UseSqlServer(configuration["Data:DemoDrinkShopProducts:ConnectionString"]);
            });
            builder.Services.AddTransient<IProductRepository, EFProductRepository>();

			builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));

			string? OAuthPath = configuration["Firebase:OauthKeyPath"],
				    bucketName = configuration["Firebase:BucketName"];
			IImageStorageService serviceForImages = new FirebaseImagesService(bucketName, OAuthPath);
			builder.Services.AddSingleton(serviceForImages);

			builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			builder.Services.AddTransient<IOrderRepository, EFOrderRepository>();
			builder.Services.AddDbContext<AppIdentityDbContext>(options =>
			{
				options.UseSqlServer(configuration["Data:DemoDrinkShopIdentity:ConnectionString"]);
			});
			builder.Services.AddIdentity<ExtendedIdentityUser, IdentityRole>()
							.AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();


			builder.Services.AddMvc(options =>
			{
				options.EnableEndpointRouting = false;
				options.ModelBinderProviders.Insert(0, new CustomDecimalModelBinderProvider());
			})
			  .AddRazorOptions(options => 
			{
                options.ViewLocationFormats.Clear(); 
                options.ViewLocationFormats.Add("/Presentation/Views/{1}/{0}.cshtml");
                options.ViewLocationFormats.Add("/Presentation/Views/Shared/{0}.cshtml");
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

                routes.MapRoute(name: null, template: "Account/Login", 
								defaults: new { controller = "Account", action = "Entry", purpose = "login" });
                routes.MapRoute(name: null, template: "Account/Register", 
								defaults: new { controller = "Account", action = "Entry", purpose = "register" });

                routes.MapRoute(name: null, template: "{controller}/{action}/{id?}");
			});

			app.Lifetime.ApplicationStopping.Register(() =>
			{
				(serviceForImages as FirebaseImagesService)?.Dispose();
			});

			SeedData.EnsurePopulated(app);
			IdentitySeedData.EnsurePopulated(app);

			app.Run();
        }
    }
}