using System.Reflection;
using Vermundo.Application.Abstractions.Messaging;
using Vermundo.Domain.Abstractions;
using Vermundo.Infrastructure;

namespace Vermundo.ArchitectureTests.Infrastructure;

public abstract class BaseTest
{
    protected static readonly Assembly DomainAssembly = typeof(Entity).Assembly;

    protected static readonly Assembly ApplicationAssembly = typeof(IBaseCommand).Assembly;

    protected static readonly Assembly InfrastructureAssembly =
        typeof(ApplicationDbContext).Assembly;

    protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
}
