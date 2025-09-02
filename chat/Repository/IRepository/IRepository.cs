using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat.Repository
{
    public interface IRepository<T> where T : class
    {
        TDest MapObject<TDest>(object source);
        IEnumerable<TDest> MapList<TDest>(IEnumerable<object> source);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task SaveAsync();
    }
}

