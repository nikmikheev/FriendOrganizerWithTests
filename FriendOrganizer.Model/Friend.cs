using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.Model
{
    public class Friend 

    {
        public Friend()
        {
            PhoneNumbers = new Collection<FriendPhoneNumber>();
            Meetings = new Collection<Meeting>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string Email{ get; set; }

        public int? FavoriteLanguageId{ get; set; }

        public ProgrammingLanguage FavoriteLanguage{ get; set; }

        public ICollection<FriendPhoneNumber> PhoneNumbers { get; set; }

        public ICollection<Meeting> Meetings { get; set; }

    }
}
