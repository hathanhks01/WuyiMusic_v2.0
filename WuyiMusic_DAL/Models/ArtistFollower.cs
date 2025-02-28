using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.Models
{
    public class ArtistFollower
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid ArtistId { get; set; }

        [Required]
        public DateTime FollowedAt { get; set; } = DateTime.Now;

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("ArtistId")]
        public virtual Artist Artist { get; set; }
    }
}
