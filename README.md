[![](https://img.shields.io/nuget/v/soenneker.semantickernel.pool.openai.azure.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.semantickernel.pool.openai.azure/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.semantickernel.pool.openai.azure/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.semantickernel.pool.openai.azure/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.semantickernel.pool.openai.azure.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.semantickernel.pool.openai.azure/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.semantickernel.pool.openai.azure/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.semantickernel.pool.openai.azure/actions/workflows/codeql.yml)

# Soenneker.SemanticKernel.Pool.OpenAi.Azure

Provides AzureOpenAI-specific registration extensions for KernelPoolManager, enabling integration with local LLMs via Semantic Kernel.

## Install

```bash
dotnet add package Soenneker.SemanticKernel.Pool.OpenAi.Azure
```

## Quick start

```csharp
using Soenneker.SemanticKernel.Pool.OpenAi.Azure;

ISemanticKernelPool pool = /* obtain from your application */;
await pool.AddAzureOpenAi("value", "value", /* supply type */ default!, "value", "value", "value", /* supply httpClientCache */ default!, 1, 1, 1, default);
```

Registers an Azure OpenAI model in the kernel pool with the specified kernel type and optional rate/token limits.

## What you get

- `SemanticKernelPoolAzureOpenAiExtension` — Provides AzureOpenAI-specific registration extensions for KernelPoolManager, enabling integration with local LLMs via Semantic Kernel.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `SemanticKernelPoolAzureOpenAiExtension.AddAzureOpenAi(pool, poolId, key, type, modelId, apiKey, endpoint, httpClientCache, rps, rpm, rpd, tokensPerDay, cancellationToken)` | Registers an Azure OpenAI model in the kernel pool with the specified kernel type and optional rate/token limits. | A task that completes when the azure open ai addition is complete. |
| `SemanticKernelPoolAzureOpenAiExtension.RemoveAzureOpenAi(pool, poolId, key, httpClientCache, cancellationToken)` | Unregisters an Azure OpenAI model from the kernel pool and removes associated HTTP client and kernel cache entries. | A task that completes when the azure open ai removal is complete. |

## Practical notes

- Cancellation stops pending work; it does not undo work that has already completed.
