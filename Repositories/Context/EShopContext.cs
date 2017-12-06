namespace Repositories.Context
{
    using Entities;
    using Microsoft.EntityFrameworkCore;

    public class EShopContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Database.EnsureCreated();
            base.OnConfiguring(optionsBuilder);
        }
    }
}