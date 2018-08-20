using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data.Repositories
{
    public interface IFriendRepository
    {
        Task<Friend> GetByIdAsync(int friendId);
        // Task<List<Friend>> GetAllAsync();
        // IEnumerable<Friend> GetAll();

        Task SaveAsync();
      //Task SaveAsync(Friend friend);

        bool HasChanges();
        void Add(Friend friend);
        void Remove(Friend friend);
    }
} 