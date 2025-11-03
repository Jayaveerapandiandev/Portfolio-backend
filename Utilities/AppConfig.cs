namespace Portfolio_Api.Utilities
{
    public static class AppConfig
    {
        private static readonly IConfigurationRoot _config;

        static AppConfig()
        {
            _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        public static string GetConnectionString()
        {
            return _config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection not found in appsettings.json.");
        }

        public static string GetEncryptionKey()
        {
            return _config["EncryptionKey"]
                ?? throw new InvalidOperationException("EncryptionKey not found in appsettings.json.");
        }

        public static IConfigurationRoot GetConfiguration() => _config;
    }
}
