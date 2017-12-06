namespace WarehouseAPI
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Repositories.Context;

    public class Startup
    {
        public IConfiguration Configuration { get; }

        #region CONSTRUCTORS

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<EShopContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("EShopDB")));


            //var connection = "Server=tcp:warehousesrv.database.windows.net,1433;Initial Catalog=WarehouseDB;Persist Security Info=False;User ID=cheetah;Password=%%JustForPad24%%;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            //var connection = "Data Source=SLAVA-PC;Initial Catalog=EShopDB;Integrated Security=True";
            //services.AddDbContext<EShopContext>(options => options.UseSqlServer(connection));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}