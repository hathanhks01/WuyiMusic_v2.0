using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.Models
{
    public class Playlist
    {
        [Key]
        public Guid PlaylistId { get; set; } = new Guid();

        [Required] 
        public string Title { get; set; }

        [Required] 
        public DateTime CreatedAt { get; set; }

        [ForeignKey("User")]
        [Required] 
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<PlaylistTrack> PlaylistTracks { get; set; }
    }
}
