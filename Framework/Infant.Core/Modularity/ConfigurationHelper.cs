using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Infant.Core.Modularity;

public class ConfigOptions
{
    public string BasePath { get; set; }
    public string EnvironmentName { get; set; }
    public string FileName { get; set; }
    public string UserSecretsId { get; set; }
    public string UserSecretsAssembly { get; set; }
    public string EnvironmentVariablesPrefix { get; set; }
    public string[] CommandLineArgs { get; set; }
}

public static class ConfigurationHelper
{
    public static IConfigurationRoot BuildConfiguration(
        ConfigOptions options,
        Action<IConfigurationBuilder> builderAction = null)
    {
        options = options ?? new ConfigOptions();
        
        if (string.IsNullOrEmpty(options.BasePath))
        {
            options.BasePath = Directory.GetCurrentDirectory();
        }

        var builder = new ConfigurationBuilder()
            .SetBasePath(options.BasePath)
            .AddJsonFile(options.FileName + ".json", optional: true, reloadOnChange: true);

        if (!string.IsNullOrEmpty(options.EnvironmentName))
        {
            builder = builder.AddJsonFile($"{options.FileName}.{options.EnvironmentName}.json", optional: true, reloadOnChange: true);
        }

        if (options.EnvironmentName == "Development")
        {
            if (options.UserSecretsId != null)
            {
                builder.AddUserSecrets(options.UserSecretsId);
            }
            else if (options.UserSecretsAssembly != null)
            {
                builder.AddUserSecrets(options.UserSecretsAssembly, true);
            }
        }

        builder = builder.AddEnvironmentVariables(options.EnvironmentVariablesPrefix);

        if (options.CommandLineArgs != null)
        {
            builder = builder.AddCommandLine(options.CommandLineArgs);
        }

        builderAction?.Invoke(builder);

        return builder.Build();
    }
}