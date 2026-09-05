using NetArchTest.Rules;
using ReserveFlow.Api.Middleware;
using ReserveFlow.Domain.Abstractions;
using ReserveFlow.Domain.Shared;

namespace ReserveFlow.Architecture.Tests;

public class LayerDependencyTests
{
    [Fact]
    public void Domain_Should_NotHaveDependencyOnOtherLayers()
    {
        var result = Types.InAssembly(typeof(Entity).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "ReserveFlow.Application",
                "ReserveFlow.Infrastructure",
                "ReserveFlow.Api")
            .GetResult();

        Assert.True(result.IsSuccessful, FormatFailures(result));
    }

    [Fact]
    public void Application_Should_NotHaveDependencyOnInfrastructureOrApi()
    {
        var result = Types.InAssembly(typeof(Application.DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "ReserveFlow.Infrastructure",
                "ReserveFlow.Api")
            .GetResult();

        Assert.True(result.IsSuccessful, FormatFailures(result));
    }

    [Fact]
    public void Api_Should_NotHaveDependencyOnDomainFeatures()
    {
        var result = Types.InAssembly(typeof(ExceptionHandlingMiddleware).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "ReserveFlow.Domain.Users",
                "ReserveFlow.Domain.Catalog",
                "ReserveFlow.Domain.Shared",
                "ReserveFlow.Domain.Exceptions")
            .GetResult();

        Assert.True(result.IsSuccessful, FormatFailures(result));
    }

    [Fact]
    public void Domain_Should_NotHaveDependencyOnEfCore()
    {
        var result = Types.InAssembly(typeof(Entity).Assembly)
            .ShouldNot()
            .HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        Assert.True(result.IsSuccessful, FormatFailures(result));
    }

    private static string FormatFailures(TestResult result) =>
        string.Join(", ", result.FailingTypes?.Select(t => t.FullName) ?? []);
}
