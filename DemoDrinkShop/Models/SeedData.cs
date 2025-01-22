using Microsoft.EntityFrameworkCore;

namespace DemoDrinkShop.Models
{
    public class SeedData
    {
        public static async void EnsurePopulated(IApplicationBuilder app)
        {
            ApplicationDbContext context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();

            context.Database.Migrate();

            if (!context.Products.Any())
            {
                context.Products.AddRange(
                    new Product
                    {
                        Name = "Cappucino",
                        Description = "This is coffee 1",
                        Category = Category.Coffee,
                        Price = 10
                    },
                    new Product
                    {
                        Name = "English Breakfast",
                        Description = "This is tea 1",
                        Category = Category.Tea,
                        Price = 5.5m
                    },
                    new Product
                    {
                        Name = "Hot Chocolate",
                        Description = "This is milk drink 1",
                        Category = Category.Milk,
                        Price = 11
                    },

                    new Product
                    {
                        Name = "Caramel Apple Spice",
                        Description = "This is milk drink 2",
                        Category = Category.Milk,
                        Price = 12.9m
                    },
                    new Product
                    {
                        Name = "Flat White",
                        Description = "This is coffee 2",
                        Category = Category.Coffee,
                        Price = 16.5m
                    },
                    new Product
                    {
                        Name = "Pumpkin Spice Latte",
                        Description = "This is coffee 3",
                        Category = Category.Coffee,
                        Price = 28
                    },

                    new Product
                    {
                        Name = "Peppermint Mocha",
                        Description = "This is coffee 4",
                        Category = Category.Coffee,
                        Price = 12.5m
                    },
                    new Product
                    {
                        Name = "Iced Espresso",
                        Description = "This is coffee 5",
                        Category = Category.Coffee,
                        Price = 17
                    },
                    new Product
                    {
                        Name = "Matcha",
                        Description = "This is tea 2",
                        Category = Category.Tea,
                        Price = 7.75m
                    }
                );
            }

            await context.SaveChangesAsync();
        }
    }
}
