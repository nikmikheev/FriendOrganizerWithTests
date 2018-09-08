using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repositories
{
    public class FriendRepository : GenericRepository<Friend, FriendOrganizerDBContext>, IFriendRepository
    {
        public FriendRepository(FriendOrganizerDBContext context) : base(context)
        {
            
        }

        public override async Task<Friend> GetByIdAsync(int friendId)
        {
            return await Context.Friends
                .Include(f=>f.PhoneNumbers)
                .SingleAsync(f =>f.Id==friendId); 
        }

        public void RemovePhoneNumber(FriendPhoneNumber model)
        {
            Context.FriendPhoneNumbers.Remove(model);
        }

        public async Task<bool> HasMeetingsAsync(int friendId)
        {
            return await Context.Meetings
                .Include(m => m.Friends)
                .AnyAsync(m => m.Friends.Any(f=>f.Id == friendId));
        }

        // public async Task SaveAsync(Friend friend)
        // Контекст уже задан, нужный параметр у клиента установлен при первом чтении друга
        //        public async Task SaveAsync()
        //{
        ////            _context.Friends.Attach(friend);
        // //            _context.Entry(friend).State = EntityState.Modified;
        //    await _context.SaveChangesAsync();
        //}


    }
}
