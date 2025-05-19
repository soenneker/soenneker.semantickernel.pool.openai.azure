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
    public static ValueTask RegisterAzureOpenAi(this ISemanticKernelPool pool, string key, KernelType type, string modelId, string apiKey, string endpoint,
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
                HttpClient httpClient = await httpClientCache.Get($"azureopenai:{modelId}", () => new HttpClientOptions
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

        return pool.Register(key, options, cancellationToken);
    }

    /// <summary>
    /// Unregisters an Azure OpenAI model from the kernel pool and removes associated HTTP client and kernel cache entries.
    /// </summary>
    public static async ValueTask UnregisterAzureOpenAi(this ISemanticKernelPool pool, string key, IHttpClientCache httpClientCache, CancellationToken cancellationToken = default)
    {
        await pool.Unregister(key, cancellationToken).NoSync();
        await httpClientCache.Remove($"azureopenai:{key}", cancellationToken).NoSync();
    }
}
