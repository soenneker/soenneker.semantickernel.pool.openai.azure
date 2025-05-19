using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.SemanticKernel.Pool.OpenAi.Azure.Tests;

[Collection("Collection")]
public class KernelPoolAzureOpenAiExtensionTests : FixturedUnitTest
{
    public KernelPoolAzureOpenAiExtensionTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }

    [Fact]
    public void Default()
    {

    }
}
