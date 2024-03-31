namespace FileWeb.App.FileCore
{
    public static class FileManagerExtensions
    {

        public static void AddFileSourceConfigConfigure(this IServiceCollection services, IConfigurationSection section)
        {
            services.Configure<FileSourceConfig>(section);
            services.AddSingleton<FileManager>();
        }

        public static void UseFileManager(this IApplicationBuilder app)
        {
            FileManager manager = app.ApplicationServices.GetService<FileManager>();
            if (manager == null)
            {
                throw new InvalidOperationException("No FileManager Service");
            }
            manager.LoadPartitionMap();
        }

    }
}
