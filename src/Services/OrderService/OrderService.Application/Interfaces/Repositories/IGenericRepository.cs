using System.Linq.Expressions;
using OrderService.Domain.SeedWork;

namespace OrderService.Application.Interfaces.Repositories;

public interface IGenericRepository<T> : IRepository<T> where T : BaseEntity, IAggregateRoot
{
    Task<List<T>> GetAll();

    Task<List<T>> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> order = null, params Expression<Func<T, object>>[] includes);
    Task<List<T>> Get(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes);
    Task<T> GetByIdAsync(Guid id);
    Task<T> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes);
    Task<T> GetSingleAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);

    Task<T> AddAsync(T entity);
    T Update(T entity);
}