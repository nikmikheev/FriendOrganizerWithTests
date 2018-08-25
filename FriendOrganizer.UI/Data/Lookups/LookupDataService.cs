using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data.Lookups
{
    public class LookupDataService : IFriendLookupDataService, 
        IProgrammingLanguageLookupDataService,
        IMeetingLookupDataService
    {
        private readonly Func<FriendOrganizerDBContext> _contextCreator;

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
                    .Select(f => new LookupItem
                    {
                        Id = f.Id,
                        DisplayMember = f.FirstName + " " + f.LastName

                    })
                    .ToListAsync();
            }
        }

        public async Task<List<LookupItem>> GetProgrammingLanguageLookupAsync()
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.ProgrammingLanguages.AsNoTracking()
                    .Select(f => new LookupItem
                    {
                        Id = f.Id,  
                        DisplayMember = f.Name

                    })
                    .ToListAsync();
            }
        }

        public async Task<List<LookupItem>> GetMeetingLookupAsync()
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.Meetings.AsNoTracking()
                    .Select(f => new LookupItem
                    {
                        Id = f.Id,
                        DisplayMember = f.Title

                    })
                    .ToListAsync();
            } 
        }

    }
}
