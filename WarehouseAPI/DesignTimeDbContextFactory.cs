namespace WarehouseAPI
{
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using Repositories.Context;

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EShopContext>
    {
        public EShopContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<EShopContext>();
            var connectionString = configuration.GetConnectionString("EShopDB");
            builder.UseSqlServer(connectionString);
            return new EShopContext(builder.Options);
        }
    }
}