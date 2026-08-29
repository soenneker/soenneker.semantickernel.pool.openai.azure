using Microsoft.SemanticKernel;
using Soenneker.Dtos.HttpClientOptions;
using Soenneker.Extensions.ValueTask;
using Soenneker.SemanticKernel.Dtos.Options;
using Soenneker.SemanticKernel.Enums.KernelType;
using Soenneker.SemanticKernel.Pool.Abstract;
using Soenneker.Utils.HttpClientCache.Abstract;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.SemanticKernel.Pool.OpenAi.Azure;

/// <summary>
/// Provides AzureOpenAI-specific registration extensions for KernelPoolManager, enabling integration with local LLMs via Semantic Kernel.
/// </summary>
public static class SemanticKernelPoolAzureOpenAiExtension
{
    /// <summary>
    /// Registers an Azure OpenAI model in the kernel pool with the specified kernel type and optional rate/token limits.
    /// </summary>
    /// <param name="pool">Pool that supplies the reusable resource.</param>
    /// <param name="poolId">Identifier of the target pool.</param>
    /// <param name="key">Key used to locate the target entry.</param>
    /// <param name="type">Runtime type to inspect or construct.</param>
    /// <param name="modelId">Identifier of the model to use.</param>
    /// <param name="apiKey">API key used to authenticate the request.</param>
    /// <param name="endpoint">Service endpoint to call.</param>
    /// <param name="httpClientCache">http Client Cache used to communicate with the external service.</param>
    /// <param name="rps">Optional requests-per-second limit.</param>
    /// <param name="rpm">Optional requests-per-minute limit.</param>
    /// <param name="rpd">Optional requests-per-day limit.</param>
    /// <param name="tokensPerDay">Optional daily token limit.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the azure open ai addition is complete.</returns>
    public static ValueTask AddAzureOpenAi(this ISemanticKernelPool pool, string poolId, string key, KernelType type, string modelId, string apiKey, string endpoint,
        IHttpClientCache httpClientCache, int? rps, int? rpm, int? rpd, int? tokensPerDay = null, CancellationToken cancellationToken = default)
    {
        var options = new SemanticKernelOptions
        {
            Type = type,
            ModelId = modelId,
            Endpoint = endpoint,
            ApiKey = apiKey,
            RequestsPerSecond = rps,
            RequestsPerMinute = rpm,
            RequestsPerDay = rpd,
            TokensPerDay = tokensPerDay,
            KernelFactory = async (opts, _) =>
            {
                // No closure: static lambda with no state needed
                HttpClient httpClient = await httpClientCache.Get($"azureopenai:{poolId}:{key}", static () => new HttpClientOptions
                {
                    Timeout = TimeSpan.FromSeconds(300)
                }, cancellationToken).NoSync();

#pragma warning disable SKEXP0010
                return type switch
                {
                    _ when type == KernelType.Chat =>
                        Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(deploymentName: opts.ModelId!, endpoint: opts.Endpoint!, apiKey: opts.ApiKey!, httpClient: httpClient),

                    _ when type == KernelType.Image =>
                        Kernel.CreateBuilder().AddAzureOpenAITextToImage(deploymentName: opts.ModelId!, endpoint: opts.Endpoint!, apiKey: opts.ApiKey!, httpClient: httpClient),

                    _ when type == KernelType.Embedding =>
                        Kernel.CreateBuilder().AddAzureOpenAIEmbeddingGenerator(deploymentName: opts.ModelId!, endpoint: opts.Endpoint!, apiKey: opts.ApiKey!, httpClient: httpClient),

                    _ => throw new NotSupportedException($"Unsupported KernelType '{type}' for Azure OpenAI registration.")
                };
#pragma warning restore SKEXP0010
            }
        };

        return pool.Add(poolId, key, options, cancellationToken);
    }

    /// <summary>
    /// Unregisters an Azure OpenAI model from the kernel pool and removes associated HTTP client and kernel cache entries.
    /// </summary>
    /// <param name="pool">Pool that supplies the reusable resource.</param>
    /// <param name="poolId">Identifier of the target pool.</param>
    /// <param name="key">Key used to locate the target entry.</param>
    /// <param name="httpClientCache">http Client Cache used to communicate with the external service.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the azure open ai removal is complete.</returns>
    public static async ValueTask RemoveAzureOpenAi(this ISemanticKernelPool pool, string poolId, string key, IHttpClientCache httpClientCache, CancellationToken cancellationToken = default)
    {
        await pool.Remove(poolId, key, cancellationToken).NoSync();
        await httpClientCache.Remove($"azureopenai:{poolId}:{key}").NoSync();
    }
}
