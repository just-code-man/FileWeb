using Serilog;
using Serilog.Events;

namespace FileWeb.App
{
    public static class AppExtension
    {
        public static IWebHostBuilder UseDefaultFileSerilog(this IWebHostBuilder builder, LogEventLevel level = LogEventLevel.Information, string outputTemplate = "{Timestamp:HH:mm:ss.FFF} || {Level} || {SourceContext:l} || {Message} || {Exception} ||end {NewLine}", string path = "logs", string title = "log", long fileSize = 1024 * 1024 * 20, string fileExt = "log", List<string> errorOverrides = null)
        {
            LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Is(level)
                .MinimumLevel.Override("Microsoft", level == LogEventLevel.Information ? LogEventLevel.Information : LogEventLevel.Error);

            if (errorOverrides != null && errorOverrides.Any())
            {
                foreach (var item in errorOverrides)
                {
                    loggerConfiguration = loggerConfiguration.MinimumLevel.Override(item, level = LogEventLevel.Error);
                }
            }

            loggerConfiguration = loggerConfiguration.Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: outputTemplate)
                .WriteTo.File(AppContext.BaseDirectory + path + $"/{title}.{fileExt}", rollingInterval: RollingInterval.Hour, outputTemplate: outputTemplate);

            Log.Logger = loggerConfiguration.CreateLogger();

            //builder.ConfigureServices((configureServices) =>
            //{
            //    configureServices.AddLogDashboard(opt =>
            //    {
            //        opt.AddAuthorizationFilter(new LogDashboardBasicAuthFilter("admin", "135246"));
            //    });
            //}).Configure(app =>
            //{
            //    app.UseLogDashboard();
            //});

            builder.UseSerilog();


            return builder;
        }
    }
}
