namespace Repositories.Context
{
    using Entities;
    using Microsoft.EntityFrameworkCore;

    public class EShopContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public EShopContext(DbContextOptions<EShopContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("My connection string");

        //    base.OnConfiguring(optionsBuilder);
        //}
    }
}