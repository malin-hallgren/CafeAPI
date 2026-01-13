
using System.Collections.Generic;

namespace CafeAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // Skapa kategorier
            var kaffeKategori = new Category { Id = 1, Name = "Kaffe" };
            var bakverkKategori = new Category { Id = 2, Name = "Bakverk" };
            var teKategori = new Category { Id = 3, Name = "Te" };

            // Skapa listan med produkter
            List<Product> cafeMeny = new List<Product>
{
    new Product
    {
        Id = 1,
        Name = "Espresso",
        Description = "En intensiv och fyllig liten kopp kaffe.",
        Price = 25.00m,
        Category = kaffeKategori
    },
    new Product
    {
        Id = 2,
        Name = "Macchiato",
        Description = "Espresso med en fläck av skummat mjölk.",
        Price = 32.00m,
        Category = kaffeKategori
    },
    new Product
    {
        Id = 3,
        Name = "Caffè Latte",
        Description = "Klassisk kaffe med mycket mjölk.",
        Price = 45.00m,
        Category = kaffeKategori
    },
    new Product
    {
        Id = 4,
        Name = "Kanelbulle",
        Description = "Hembakad bulle med massor av kanel och socker.",
        Price = 35.00m,
        Category = bakverkKategori
    },
    new Product
    {
        Id = 5,
        Name = "Chokladboll",
        Description = "Klassisk svensk chokladboll rullad i pärlsocker.",
        Price = 20.00m,
        Category = bakverkKategori
    },
           
    // Teprodukter (Nya)
    new Product
    {
        Id = 6,
        Name = "Earl Grey",
        Description = "Klassiskt svart te smaksatt med bergamott.",
        Price = 28.00m,
        Category = teKategori
    },
    new Product
    {
        Id = 7,
        Name = "Grönt te (Sencha)",
        Description = "Friskt japanskt grönt te.",
        Price = 30.00m,
        Category = teKategori
    },
    new Product
    {
        Id = 8,
        Name = "Chai Latte",
        Description = "Kryddigt te med ångad mjölk och honung.",
        Price = 42.00m,
        Category = teKategori
    }
};

            // Put Products

            app.MapPut("/produkter/{id}", (int id, Product P ) =>
            {
                var produkt = cafeMeny.Find(p => p.Id == id);
                if (produkt == null)
                {
                    return Results.NotFound();
                }
                produkt.Name = P.Name;
                produkt.Description = P.Description;
                produkt.Price = P.Price;
                produkt.Category = P.Category;
                return Results.Ok(produkt);

            });
           //GET all categories

            app.MapGet("/kategorier", () =>
            {
                var categories = new List<Category> { kaffeKategori, bakverkKategori, teKategori };
                return Results.Ok(categories);
            });
            //Get catagory by id

            app.MapGet("kategorier/{id}", (int id) =>
            {
               

            })
            //post categories

            app.Run();
        }
    }
}
