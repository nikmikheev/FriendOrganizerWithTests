using System.Collections.Generic;
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

            context.ProgrammingLanguages.AddOrUpdate(
                pl=>pl.Name,
                new ProgrammingLanguage { Name = "SQL" },
                new ProgrammingLanguage { Name = "JavaScript" },
                new ProgrammingLanguage { Name = "C#" },
                new ProgrammingLanguage { Name = "Java" },
                new ProgrammingLanguage { Name = "Python" },
                new ProgrammingLanguage { Name = "Swift" }
                );

            context.SaveChanges();

            context.FriendPhoneNumbers.AddOrUpdate(
                ph=> ph.Number,
                new FriendPhoneNumber {Number = "+64 1231234", FriendId = context.Friends.First().Id});

            context.Meetings.AddOrUpdate(
                m=>m.Title,
                new Meeting
                {
                    Title = "Meeting C#",
                    DateFrom = new DateTime(2018, 08, 20),
                    DateTo = new DateTime(2018,08,24),
                    Friends = new List<Friend>
                    {
                        context.Friends.Single(f=>f.FirstName == "Thomas" && f.LastName == "Huber" ),
                        context.Friends.Single(f=>f.FirstName == "Nikolay" && f.LastName == "Mikheeev")
                    }

                }
                );

        }
    }
}
