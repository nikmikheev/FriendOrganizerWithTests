using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data.Repositories
{
    public class MeetingRepository : GenericRepository<Meeting, FriendOrganizerDBContext>, 
                  IMeetingRepository
    {
        public MeetingRepository(FriendOrganizerDBContext context) : base(context)
        {
            
        }

        public override async Task<Meeting> GetByIdAsync(int id)
        {
            return await Context.Meetings
                .Include(m => m.Friends)
                .SingleAsync(m=>m.Id==id); 
        }

        public async Task<List<Friend>> GetAllFriendsAsync()
        {
            return await Context.Set<Friend>().ToListAsync();
        }

        public async Task RefreshFriendAsync(int friendId)
        {
            var DBEntityEntry = Context.ChangeTracker.Entries<Friend>()
                .SingleOrDefault(db => db.Entity.Id == friendId);
           if (DBEntityEntry != null)
            {
                await DBEntityEntry.ReloadAsync();
            }
        }
    }
}
