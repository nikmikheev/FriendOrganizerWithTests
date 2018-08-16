using FriendOrganizer.Model;

namespace FriendOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FriendOrganizer.DataAccess.FriendOrganizerDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FriendOrganizer.DataAccess.FriendOrganizerDBContext context)
        {
            //  This method will be called after migrating to the latest version.
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            context.Friends.AddOrUpdate(
                    f => f.FirstName,
                    new Friend { FirstName = "Thomas", LastName = "Huber" },
                    new Friend { FirstName = "Andreas", LastName = "Boehler" },
                    new Friend { FirstName = "Julia", LastName = "Huber" },
                    new Friend { FirstName = "Sara", LastName = "Egin" },
                    new Friend { FirstName = "Nikolay", LastName = "Mikheeev", Email = "n.mikheev@magicsoft.ru"}
                    
                );
        }
    }
}
