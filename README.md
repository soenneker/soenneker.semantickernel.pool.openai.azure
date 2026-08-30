[![](https://img.shields.io/nuget/v/soenneker.semantickernel.pool.openai.azure.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.semantickernel.pool.openai.azure/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.semantickernel.pool.openai.azure/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.semantickernel.pool.openai.azure/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.semantickernel.pool.openai.azure.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker/soenneker.semantickernel.pool.openai.azure/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.semantickernel.pool.openai.azure/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.semantickernel.pool.openai.azure/actions/workflows/codeql.yml)

# Soenneker.SemanticKernel.Pool.OpenAi.Azure

Azure OpenAI connector registration helpers for `Soenneker.SemanticKernel.Pool`.

## Installation

```bash
dotnet add package Soenneker.SemanticKernel.Pool.OpenAi.Azure
```

## Add an Azure OpenAI entry

Resolve the pool and HTTP client cache from dependency injection, then register an Azure deployment:

```csharp
using Soenneker.SemanticKernel.Enums.KernelType;
using Soenneker.SemanticKernel.Pool.OpenAi.Azure;

await pool.AddAzureOpenAi(
    poolId: "chat",
    key: "azure-primary",
    type: KernelType.Chat,
    modelId: "chat-deployment-name",
    apiKey: configuration["AzureOpenAI:ApiKey"]!,
    endpoint: configuration["AzureOpenAI:Endpoint"]!,
    httpClientCache: httpClientCache,
    rps: 2,
    rpm: 60,
    rpd: 1_000,
    tokensPerDay: null,
    cancellationToken);
```

Despite the parameter name, `modelId` is passed to Semantic Kernel as the Azure deployment name.

Supported types are:

- `KernelType.Chat` for chat completion
- `KernelType.Image` for text-to-image
- `KernelType.Embedding` for embedding generation

Other types throw `NotSupportedException` when the pool first constructs the kernel.

Every connector receives the supplied Azure endpoint, API key, and a shared HTTP client cached under `azureopenai:{poolId}:{key}` with a five-minute timeout.

Pool quota values are reservations made when `GetAvailable` selects the entry. `tokensPerDay` counts one unit per acquisition; it is not populated from Azure usage data.

## Remove the entry

Use the matching helper so both the pool entry and cached HTTP client are removed:

```csharp
await pool.RemoveAzureOpenAi(
    "chat",
    "azure-primary",
    httpClientCache,
    cancellationToken);
```

Keep the API key in a protected configuration provider and avoid logging or serializing the generated `SemanticKernelOptions`.
