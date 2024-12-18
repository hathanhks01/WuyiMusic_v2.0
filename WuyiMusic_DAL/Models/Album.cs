using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.Models
{
    public class Album
    {
        [Key]
        public Guid AlbumId { get; set; } = Guid.NewGuid();

        [Required] 
        public string Title { get; set; }

        [Required] 
        public DateTime ReleaseDate { get; set; }

        [ForeignKey("Artist")]
        [Required] 
        public Guid ArtistId { get; set; }
        public virtual Artist Artist { get; set; }

        public virtual ICollection<Track> Tracks { get; set; }
    }

}
