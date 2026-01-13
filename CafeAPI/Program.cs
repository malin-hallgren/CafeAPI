using CafeAPI.Data;
using CafeAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;

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

            builder.Services.AddDbContext<CafeAPIDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("CafeDatabase"));
            });

            var context = new CafeAPIDbContext(
                builder.Services.BuildServiceProvider().GetRequiredService<DbContextOptions<CafeAPIDbContext>>());

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // Skapa kategorier

            if (context.Categories.ToList().Count < 1)
            {
                var kaffeKategori = new Category { Name = "Kaffe" };
                var bakverkKategori = new Category { Name = "Bakverk" };
                var teKategori = new Category { Name = "Te" };

                context.Categories.Add(kaffeKategori);
                context.Categories.Add(bakverkKategori);
                context.Categories.Add(teKategori);

                List<Product> cafeMeny = new List<Product>
            {
                new Product
                {
                    Name = "Espresso",
                    Description = "En intensiv och fyllig liten kopp kaffe.",
                    Price = 25.00m,
                    Category = kaffeKategori
                },
                new Product
                {
                    Name = "Macchiato",
                    Description = "Espresso med en fläck av skummat mjölk.",
                    Price = 32.00m,
                    Category = kaffeKategori
                },
                new Product
                {
                    Name = "Caffè Latte",
                    Description = "Klassisk kaffe med mycket mjölk.",
                    Price = 45.00m,
                    Category = kaffeKategori
                },
                new Product
                {
                    Name = "Kanelbulle",
                    Description = "Hembakad bulle med massor av kanel och socker.",
                    Price = 35.00m,
                    Category = bakverkKategori
                },
                new Product
                {
                    Name = "Chokladboll",
                    Description = "Klassisk svensk chokladboll rullad i pärlsocker.",
                    Price = 20.00m,
                    Category = bakverkKategori
                },

                // Teprodukter (Nya)
                new Product
                {
                    Name = "Earl Grey",
                    Description = "Klassiskt svart te smaksatt med bergamott.",
                    Price = 28.00m,
                    Category = teKategori
                },
                new Product
                {
                    Name = "Grönt te (Sencha)",
                    Description = "Friskt japanskt grönt te.",
                    Price = 30.00m,
                    Category = teKategori
                },
                new Product
                {
                    Name = "Chai Latte",
                    Description = "Kryddigt te med ångad mjölk och honung.",
                    Price = 42.00m,
                    Category = teKategori
                }
            };

                context.Products.AddRange(cafeMeny);
            }
            

            //List<Category> kategorier = new List<Category>
            //{
            //    kaffeKategori,
            //    bakverkKategori,
            //    teKategori
            //};

            // Skapa listan med produkter
            

            context.SaveChanges();

            app.MapGet("/produkter", () =>
            {
                return context.Products.Include(p => p.Category).ToList(); ;
            });

            app.MapPost("/produkter", (Product product) =>
            {
                var cafeMeny = context.Products.ToList();
                //product.Id = cafeMeny.Any() ? cafeMeny.Max(p => p.Id) + 1 : 1;

                if (product.Category != null)
                {
                    // If category has an Id, use it to find the existing category
                    if (product.Category.Id > 0)
                    {
                        var existingCategory = context.Categories.FirstOrDefault(c => c.Id == product.Category.Id);
                        if (existingCategory != null)
                        {
                            product.Category = existingCategory;
                        }
                    }
                    // If category has a Name but no Id, find by name
                    else if (!string.IsNullOrEmpty(product.Category.Name))
                    {
                        var existingCategory = context.Categories.FirstOrDefault(c => c.Name == product.Category.Name);
                        if (existingCategory != null)
                        {
                            product.Category = existingCategory;
                        }
                    }
                }

                context.Products.Add(product);
                context.SaveChanges();
                return product;
            });

            app.MapDelete("/produkter/{id}", (int id) =>
            {
                //var cafeMeny = context.Products.Include(p => p.Category).ToList();
                var product = context.Products.FirstOrDefault(p => p.Id == id);
                if (product == null)
                {
                    return Results.NotFound();
                }

                context.Products.Remove(product);
                context.SaveChanges();
                return Results.Ok();
            });

   

            app.MapPut("/produkter/{id}", (int id, Product P ) =>
            {
                var product = context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);
                if (product == null)
                {
                    return Results.NotFound();
                }
                
                product.Name = P.Name;
                product.Description = P.Description;
                product.Price = P.Price;

                if (P.Category != null && P.Category.Id > 0)
                {
                    var trackedCategory = context.Categories.Local.FirstOrDefault(c => c.Id == P.Category.Id);
                    if (trackedCategory == null)
                    {
                        context.Attach(P.Category);
                        product.Category = P.Category;
                    }
                    else
                    {
                        product.Category = trackedCategory;
                    }
                }
                
                context.SaveChanges();
                return Results.Ok(product);

            });
         

            app.MapGet("/kategorier", () =>
            {
                return context.Categories.ToList();
            });
         

            app.MapGet("kategorier/{id}", (int id) =>
            {
                var kategorier = context.Categories.ToList();
                var kategori = kategorier.FirstOrDefault(p => p.Id == id);
                if (kategori != null)
                {
                    context.SaveChanges();
                    return Results.Ok(kategori);
                }
                return Results.NotFound();

            });
        

            app.MapPost("/kategorier", (Category category) =>
            {
                var kategorier = context.Categories.ToList();
                //category.Id = kategorier.Any() ? kategorier.Max(p => p.Id) + 1 : 1;
                context.Categories.Add(category);

                context.SaveChanges();
                return category;
            });

            app.MapGet("/produkter/kategorier/{categoryId}", (int categoryId) =>
            {
                var cafeMeny = context.Products.Include(p => p.Category).ToList();
                var produkterIKategori = cafeMeny.Where(p => p.Category != null && p.Category.Id == categoryId).ToList();
                context.SaveChanges();
                return produkterIKategori;
            });

            app.Run();
        }
    }
}
