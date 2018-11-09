using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsersControl.Models
{
    [Table("AspNetUsers")]
    public class ApplicationUser : IdentityUser
    {
        [Column("Surname")]
        public string Surname { get; set; }

        [Column("FirstName")]
        public string FirstName { get; set; }

        [Column("LastName")]
        public string LastName { get; set; }
    }
}
