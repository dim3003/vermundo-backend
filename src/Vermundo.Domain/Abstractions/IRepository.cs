namespace Vermundo.Domain.Abstractions;

public interface IRepository<T>
    where T : Entity
{
    Task AddAsync(T entity);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Remove(T entity);
}
