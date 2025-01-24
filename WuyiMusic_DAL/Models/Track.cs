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

        public string? Title { get; set; }

        public TimeSpan? Duration { get; set; }
        public string? TrackImage { get; set; }

        [ForeignKey("Album")]
     
        public Guid? AlbumId { get; set; }
        public virtual Album? Album { get; set; }

        [ForeignKey("Artist")]
     
        public Guid? ArtistId { get; set; }
        public virtual Artist? Artist { get; set; }

        public string? FilePath { get; set; }
        public int? Likes { get; set; }
        public int? ListenCount { get; set; } = 0; // Số lượt nghe
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<PlaylistTrack>? PlaylistTracks { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Rating>? Ratings { get; set; }
        public virtual ICollection<Lyrics>? Lyrics { get; set; }
    }
}
