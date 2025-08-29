namespace HufmannCompressor.WinFormsApp
{
    using HuffmanCompressor.Lib;
    using Microsoft.Extensions.DependencyInjection;

    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            var serviceProvider = ConfigureServices();
            var compressor = serviceProvider.GetRequiredService<IFileCompressor>();
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm(compressor));
        }

        /// <summary>
        /// Configures dependency injection container.
        /// </summary>
        /// <returns>Service provider with registered services.</returns>
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Register IFileCompressor with HuffmanCompressor
            services.AddTransient<IFileCompressor, HuffmanCompressor>();
            services.AddTransient<IFrequencyCounter, FrequencyCounter>();
            services.AddTransient<IBitReader, BitReader>();
            services.AddTransient<IBitWriter, BitWriter>();
            return services.BuildServiceProvider();
        }
    }
}