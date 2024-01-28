namespace OnlineShop.Library.Common.Interfaces
{
    public interface IRepo<T>
    {
        Task<Guid> AddAsync(T entity);
        Task<IEnumerable<Guid>> AddRangeAsync(IEnumerable<T> entities);
        Task<int> DeleteAsync(Guid id);
        Task<int> DeleteRangeAsync(IEnumerable<Guid> ids);
        Task<int> SaveAsync(T entity);
        Task<T> GetOneAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
    }
}
