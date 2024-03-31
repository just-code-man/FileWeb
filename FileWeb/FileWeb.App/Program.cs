using Autofac.Extensions.DependencyInjection;

namespace FileWeb.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);
            builder.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.ConfigureWebHostDefaults(
                builder =>
                builder
                .UseDefaultFileSerilog()
                .UseStartup<Startup>()
                );

            var app = builder.Build();

            app.Run();
        }
    }
}