using Soenneker.Tests.HostedUnit;

namespace Soenneker.SemanticKernel.Pool.OpenAi.Azure.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class KernelPoolAzureOpenAiExtensionTests : HostedUnitTest
{
    public KernelPoolAzureOpenAiExtensionTests(Host host) : base(host)
    {
    }

    [Test]
    public void Default()
    {

    }
}
