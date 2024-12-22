using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WuyiMusic_DAL.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; } = Guid.NewGuid();

        [Required] 
        public string Username { get; set; }

        [Required] 
        [EmailAddress] 
        public string Email { get; set; }

        [Required] 
        public string Password { get; set; }

        public string? ProfileImage { get; set; } 

        [Required] 
        public DateTime? CreatedAt { get; set; }

        [Required] 
        public bool IsPremium { get; set; }

        public virtual ICollection<UserRole>? UserRoles { get; set; }
        public virtual ICollection<Playlist>? Playlists { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Rating>? Ratings { get; set; }
        public virtual ICollection<Suggestion>? Suggestions { get; set; }
    }
}
