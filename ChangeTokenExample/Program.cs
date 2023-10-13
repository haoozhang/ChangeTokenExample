namespace ChangeTokenExample
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            InitializeHost();
        }

        private static void InitializeHost()
        {
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(builder =>
                {
                    // use startup
                    builder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .Build()
                .Run();
        }
    }
}