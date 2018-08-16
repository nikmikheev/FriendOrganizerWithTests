using System.Collections.Generic;
using System.Threading.Tasks;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data
{
    public interface ILookupDataService
    {
        Task<List<LookupItem>> GetFriendLookupAsync();
    }
}