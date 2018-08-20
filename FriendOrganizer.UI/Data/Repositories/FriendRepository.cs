using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repositories
{
    public class FriendRepository : IFriendRepository
    {
        private FriendOrganizerDBContext _context;

        public FriendRepository(FriendOrganizerDBContext context)
        {
            _context = context;
        }

        public void Add(Friend friend)
        {
            _context.Friends.Add(friend);
                
        }

        public async Task<Friend> GetByIdAsync(int friendId)
        {
//            yield return new Friend { FirstName = "Thomas", LastName = "Huber" };
            return await _context.Friends.SingleAsync(f=>f.Id==friendId); 
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }

        public void Remove(Friend friend)
        {
           _context.Friends.Remove(friend);
        }

        //        public async Task SaveAsync(Friend friend)
        // Контекст уже задан, нужны йпараметру клиента установлен при первом чтении друга
        public async Task SaveAsync()
        {
            //            _context.Friends.Attach(friend);
            //            _context.Entry(friend).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
