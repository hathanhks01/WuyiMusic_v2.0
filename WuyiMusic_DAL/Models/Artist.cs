using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.Models
{
    public class Artist
    {
        [Key]
        public Guid ArtistId { get; set; } = Guid.NewGuid();
        [Required]
        public string? Name { get; set; }
        public string? Bio { get; set; }
        public string? ArtistImage { get; set; }
        public string? MetaLink { get; set; }
        public bool? IsVerified { get; set; } = false;

        [ForeignKey(nameof(ArtistId))]
        public Guid? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public virtual ICollection<Album>? Albums { get; set; }
        public virtual ICollection<Track>? Tracks { get; set; }
        public virtual ICollection<ArtistFollower> Followers { get; set; }
        public virtual User? User { get; set; }
    }
}
