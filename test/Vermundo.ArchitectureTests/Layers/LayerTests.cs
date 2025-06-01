using Vermundo.ArchitectureTests.Infrastructure;
using FluentAssertions;
using NetArchTest.Rules;

namespace Vermundo.ArchitectureTests.Layers;

public class LayerTests : BaseTest
{
    [Fact]
    public void DomainLayer_ShouldNotHaveDependenciesOn_ApplicationLayer()
    {
        var result = Types
            .InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOnAny(ApplicationAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void DomainLayer_ShouldNotHaveDependenciesOn_InfrastructureLayer()
    {
        var result = Types
            .InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOnAny(InfrastructureAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void ApplicationLayer_ShouldNotHaveDependenciesOn_InfrastructureLayer()
    {
        var result = Types
            .InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOnAny(InfrastructureAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void ApplicationLayer_ShouldNotHaveDependenciesOn_PresentationLayer()
    {
        var result = Types
            .InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOnAny(PresentationAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void InfrastructureLayer_ShouldNotHaveDependenciesOn_PresentationLayer()
    {
        var result = Types
            .InAssembly(InfrastructureAssembly)
            .Should()
            .NotHaveDependencyOnAny(PresentationAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
