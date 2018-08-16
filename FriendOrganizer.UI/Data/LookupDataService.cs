using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data
{
    public class LookupDataService : ILookupDataService
    {
        private Func<FriendOrganizerDBContext> _contextCreator;

        public LookupDataService(Func<FriendOrganizerDBContext> contextCreator)
        {
            _contextCreator = contextCreator;
        }
        public async Task<List<LookupItem>> GetFriendLookupAsync()
        {
            //            yield return new Friend { FirstName = "Thomas", LastName = "Huber" };
            using (var ctx = _contextCreator())
            {
                return await ctx.Friends.AsNoTracking()
                    .Select(f=> new LookupItem
                    {
                        Id = f.Id,
                        DisplayMember= f.FirstName + " " + f.LastName

                    })
                    .ToListAsync();
            }


        }
    }
}
