namespace WarehouseAPI
{
    using Controllers;
    using DTOs;
    using DTOs.Creational;
    using DTOs.Gettable;
    using DTOs.Patchable;
    using DTOs.Updatable;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using OutputFormatters.Internal;
    using OutputFormatters.Yaml;
    using Repositories.Context;
    using Repositories.Entities;
    using Repositories.Extensions;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

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
            services.AddMvc(options =>
            {
                options.RespectBrowserAcceptHeader = true;

                options.InputFormatters.Add(new XmlSerializerInputFormatter());
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());

                options.InputFormatters.Add(new YamlInputFormatter(new DeserializerBuilder()
                    .WithNamingConvention(new CamelCaseNamingConvention()).Build()));
                options.OutputFormatters.Add(new YamlOutputFormatter(new SerializerBuilder()
                    .WithNamingConvention(new CamelCaseNamingConvention()).Build()));
                options.FormatterMappings.SetMediaTypeMappingForFormat("yaml", MediaTypeHeaderValues.ApplicationYaml);
            });



#if DEBUG
            services.AddDbContext<EShopContext>(options =>
            {

                options.UseSqlServer(Configuration.GetConnectionString("EShopDBLocal"));
            });
#endif
            
#if RELEASE
            services.AddDbContext<EShopContext>(options =>
            {

                options.UseSqlServer(Configuration.GetConnectionString("EShopDbRemote"));
            });
#endif
            //var connection = "Server=tcp:warehousesrv.database.windows.net,1433;Initial Catalog=WarehouseDB;Persist Security Info=False;User ID=cheetah;Password=%%JustForPad24%%;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            //var connection = "Data Source=SLAVA-PC;Initial Catalog=EShopDB;Integrated Security=True";
            //services.AddDbContext<EShopContext>(options => options.UseSqlServer(connection));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, EShopContext eShopContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }

            app.UseStatusCodePages();

            app.UseMvc();

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Product, ProductToGetDTO>();
                cfg.CreateMap<ProductToCreateDTO, Product>();
                cfg.CreateMap<ProductToUpdateDTO, Product>();
                cfg.CreateMap<Product, ProductToPatchDTO>();

                cfg.CreateMap<Customer, CustomerToGetDTO>();
                cfg.CreateMap<CustomerToCreateDTO, Customer>();
                cfg.CreateMap<CustomerToUpdateDTO, Customer>();
                cfg.CreateMap<Customer, CustomerToPatchDTO>();

                cfg.CreateMap<Order, OrderToGetDTO>();
                cfg.CreateMap<OrderDetail, OrderDetailToGetDTO>();
            });

            eShopContext.EnsureSeedDataForProducts();
            eShopContext.EnsureSeedDataForCustomers();
            eShopContext.EnsureSeedDataForOrders();
        }
    }
}