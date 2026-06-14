using System.Reflection;
using NetArchTest.Rules;
using Xunit;

namespace Members.Domain.UnitTests;

public class ArchitectureTests
{
    private static readonly Assembly DomainAssembly = Assembly.Load("Members.Domain");
    private static readonly Assembly ApplicationAssembly = Assembly.Load("Members.Application");
    private static readonly Assembly InfrastructureAssembly = Assembly.Load("Members.Infrastructure");

    [Fact]
    public void Domain_ShouldNotDependOnApplication()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn("Members.Application")
            .GetResult();

        Assert.True(result.IsSuccessful, "Domain should not depend on Application layer.");
    }

    [Fact]
    public void Domain_ShouldNotDependOnInfrastructure()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn("Members.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful, "Domain should not depend on Infrastructure layer.");
    }

    [Fact]
    public void Domain_ShouldNotDependOnWebApi()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn("Members.WebApi")
            .GetResult();

        Assert.True(result.IsSuccessful, "Domain should not depend on WebApi layer.");
    }

    [Fact]
    public void Application_ShouldNotDependOnInfrastructure()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("Members.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful, "Application should not depend on Infrastructure layer.");
    }

    [Fact]
    public void Application_ShouldNotDependOnWebApi()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("Members.WebApi")
            .GetResult();

        Assert.True(result.IsSuccessful, "Application should not depend on WebApi layer.");
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOnWebApi()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOn("Members.WebApi")
            .GetResult();

        Assert.True(result.IsSuccessful, "Infrastructure should not depend on WebApi layer.");
    }
}
