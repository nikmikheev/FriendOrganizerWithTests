﻿using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data
{
    public class FriendDataService : IFriendDataService
    {
        private Func<FriendOrganizerDBContext> _contextCreator;

        public FriendDataService(Func<FriendOrganizerDBContext> contextCreator)
        {
            _contextCreator = contextCreator;
        }
        public async Task<Friend> GetByIdAsync(int friendId)
        {
//            yield return new Friend { FirstName = "Thomas", LastName = "Huber" };
            using (var ctx = _contextCreator())
            {
                return await ctx.Friends.AsNoTracking().SingleAsync(f=>f.Id==friendId); 
            }
        }

        public async Task SaveAsync(Friend friend)
        {
            using (var context = _contextCreator())
            {
                context.Friends.Attach(friend);
                context.Entry(friend).State = EntityState.Modified;
                await context.SaveChangesAsync();

            };
        }
    }
}
