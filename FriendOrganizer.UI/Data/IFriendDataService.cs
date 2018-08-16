using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data
{
    public interface IFriendDataService
    {
        Task<Friend> GetByIdAsync(int friendId);
        // Task<List<Friend>> GetAllAsync();
        // IEnumerable<Friend> GetAll();
    }
} 