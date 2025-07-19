using System.Reflection;
using Vermundo.ArchitectureTests.Infrastructure;
using Vermundo.Domain.Abstractions;
using FluentAssertions;
using NetArchTest.Rules;

namespace Vermundo.ArchitectureTests.Domain;

public class DomainTests : BaseTest
{
    [Fact]
    public void Entities_ShouldHave_PrivateParameterLessConstructor()
    {
        IEnumerable<Type> entityTypes = Types
            .InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(Entity))
            .GetTypes();

        var failingTypes = new List<Type>();
        foreach (Type entityType in entityTypes)
        {
            ConstructorInfo[] constructors = entityType.GetConstructors(
                BindingFlags.NonPublic | BindingFlags.Instance
            );

            if (!constructors.Any(c => c.IsPrivate && c.GetParameters().Length == 0))
            {
                failingTypes.Add(entityType);
            }
        }

        failingTypes.Should().BeEmpty();
    }
}
