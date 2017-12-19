namespace WarehouseAPI
{
    using HibernatingRhinos.Profiler.Appender.EntityFramework;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            EntityFrameworkProfiler.Initialize();
#endif
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}