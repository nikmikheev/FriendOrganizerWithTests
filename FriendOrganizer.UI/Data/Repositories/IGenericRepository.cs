using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repositories
{
    public interface IGenericRepository<T>
    {
        Task<T> GetByIdAsync(int Id);
        Task SaveAsync();
        bool HasChanges();
        void Add(T item);
        void Remove(T item);
    }
}