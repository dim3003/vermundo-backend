using Vermundo.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Vermundo.Infrastructure;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
