using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using OpenAI;
using System.ClientModel;

namespace AIStreaming;

public static class OpenAIExtensions
{
    public static IServiceCollection AddAzureOpenAI(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .Configure<AzureOpenAIOptions>(configuration.GetSection("AzureOpenAI"))
            .AddSingleton<AzureOpenAIClient>(provider =>
            {
                var options = provider.GetRequiredService<IOptions<AzureOpenAIOptions>>().Value;

                ArgumentException.ThrowIfNullOrWhiteSpace(options.Endpoint);
                ArgumentException.ThrowIfNullOrWhiteSpace(options.Key);
                ArgumentException.ThrowIfNullOrWhiteSpace(options.DeploymentName);

                return new AzureOpenAIClient(
                    new Uri(options.Endpoint),
                    new ApiKeyCredential(options.Key),
                    new AzureOpenAIClientOptions());
            });
    }

    public static IServiceCollection AddOpenAI(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .Configure<OpenAIOptions>(configuration.GetSection("OpenAI"))
            .AddSingleton<OpenAIClient>(provider =>
            {
                var options = provider.GetRequiredService<IOptions<OpenAIOptions>>().Value;

                ArgumentException.ThrowIfNullOrWhiteSpace(options.Key);

                return new OpenAIClient(new ApiKeyCredential(options.Key));
            });
    }
}
