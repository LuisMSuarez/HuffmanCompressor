namespace HuffmanCompressor.Cmd;

using System;
using Microsoft.Extensions.DependencyInjection;
using HuffmanCompressor.Lib;

/// <summary>
/// Main entry point for the HuffmanCompressorCmd console application.
/// </summary>
public class Program
{
    private IFileCompressor _compressor;

    /// <summary>
    /// Constructor of the Program class.
    /// </summary>
    public Program(IFileCompressor compressor)
    {
        _compressor = compressor;
    }

    /// <summary>
    /// The main entry point (Main method) for the application.
    /// </summary>
    /// <param name="args">Program arguments.</param>
    public static void Main(string[] args)
    {
        var serviceProvider = ConfigureServices();
        var program = new Program(serviceProvider.GetRequiredService<IFileCompressor>());
        program.Run(args);
    }

    /// <summary>
    /// The main entry point (Main method) for the application always needs to be declared as static.
    /// That means that it cannot access non-static members of the class, such as _compressor.
    /// Declaring the _compressor member as static would create a single instance of the compressor for all instances of the Program class.
    /// This would be a problem if the Program class was used in a multi-threaded environment, such as when unit tests are run, where we
    /// may run parallel compression jobs or even want to inject a mock compressor for testing purposes.
    /// To avoid this, we create the Run wrapper as non-static and have Main create an instance of the Program class to invoke it.
    /// </summary>
    /// <param name="args">Program args</param>
    /// <exception cref="ArgumentException"></exception>
    public void Run(string[] args)
    {
        const string usageString = "Usage: [compress|inflate] [input file path] [output file path]";

        if (args.Length != 3)
        {
            Console.WriteLine(usageString);
            throw new ArgumentException(usageString);
        }

        switch (args[0].ToLower())
        {
            case "compress":
                _compressor.Compress(args[1], args[2]);
                break;
            case "inflate":
                _compressor.Inflate(args[1], args[2]);
                break;
            default:
                Console.WriteLine(usageString);
                throw new ArgumentException(usageString);
        }
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

        return services.BuildServiceProvider();
    }
}