namespace AIStreaming;

public sealed class AzureOpenAIOptions
{
    /// <summary>
    /// The endpoint of Azure OpenAI service.
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// The key of OpenAI service.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// The model to use.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// The deployment name of the model.
    /// </summary>
    public string? DeploymentName { get; set; }
}

public sealed class OpenAIOptions
{
    /// <summary>
    /// The endpoint of OpenAI service.
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// The key of OpenAI service.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// The model to use.
    /// </summary>
    public string? Model { get; set; }
}
