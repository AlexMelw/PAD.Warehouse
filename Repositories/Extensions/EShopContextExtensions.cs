namespace Repositories.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Context;
    using Entities;

    public static class EShopContextExtensions
    {
        public static void EnsureSeedDataForContext(this EShopContext context)
        {
            if (context.Products.Any())
            {
                return;
            }

            var products = new List<Product>
            {
                new Product
                {
                    ImageUri = "http://icons.iconarchive.com/icons/mdgraphs/iphone-4g/512/iPhone-4G-shadow-icon.png",
                    Price = 1399.99M,
                    Label = "iPhone X - Apple",
                    Available = true
                },
                new Product
                {
                    ImageUri = "http://icons.iconarchive.com/icons/dailyoverview/tv/256/television-06-icon.png",
                    Price = 999.99M,
                    Label = "Samsung Smart TV",
                    Available = true
                },
                new Product
                {
                    Price = 399.99M,
                    ImageUri = "http://icons.iconarchive.com/icons/dapino/summer-blue/512/Fan-icon.png",
                    Label = "XXX Fan",
                    Available = true
                },
                new Product
                {
                    Price = 1399.99M,
                    Label = "Magic wand",
                    ImageUri =
                        "http://icons.iconarchive.com/icons/custom-icon-design/flatastic-6/512/Magic-wand-icon.png",
                    Available = false
                },
                new Product
                {
                    Price = 14.99M,
                    Label = "Funky Soccer Ball",
                    ImageUri = "http://icons.iconarchive.com/icons/martin-berube/sport/256/Soccer-icon.png",
                    Available = true
                },
                new Product
                {
                    Price = 4.99M,
                    Label = "Teddy bear",
                    ImageUri = "http://icons.iconarchive.com/icons/custom-icon-design/flatastic-10/512/Bear-icon.png",
                    Available = true
                },
                new Product
                {
                    Price = 123.78M,
                    Label = "Classic Watch",
                    ImageUri = "http://icons.iconarchive.com/icons/r34n1m4ted/chanel/512/WATCH-icon.png",
                    Available = true
                },
                new Product
                {
                    Price = 4.99M,
                    Label = "Walking with dinosaurs DVD",
                    ImageUri =
                        "http://icons.iconarchive.com/icons/firstline1/movie-mega-pack-5/512/Walking-with-Dinosaurs-icon.png",
                    Available = true
                },
                new Product
                {
                    Price = 12.99M,
                    Label = "ADATA USB Stick 128 GB",
                    ImageUri =
                        "http://icons.iconarchive.com/icons/jonathan-rey/device/256/Kingston-DataTraveler-USB-Stick-icon.png",
                    Available = true
                },
                new Product
                {
                    Price = 1.99M,
                    Label = "ICONIX MICRO SD 16 GB",
                    ImageUri = "http://icons.iconarchive.com/icons/dakirby309/simply-styled/128/Micro-SD-Card-icon.png",
                    Available = true
                }
            };

            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}