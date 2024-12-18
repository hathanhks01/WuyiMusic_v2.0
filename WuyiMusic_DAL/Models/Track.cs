using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WuyiMusic_DAL.Models
{
    public class Track
    {
        [Key]
        public Guid TrackId { get; set; } = Guid.NewGuid();

        [Required]
        public string Title { get; set; }

        [Required] 
        public TimeSpan Duration { get; set; }

        [ForeignKey("Album")]
        [Required]
        public Guid AlbumId { get; set; }
        public virtual Album Album { get; set; }

        [ForeignKey("Artist")]
        [Required]
        public Guid ArtistId { get; set; }
        public virtual Artist Artist { get; set; }

        public string FilePath { get; set; }
        public int Likes { get; set; }

        public virtual ICollection<PlaylistTrack> PlaylistTracks { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<Lyrics> Lyrics { get; set; }
    }
}
