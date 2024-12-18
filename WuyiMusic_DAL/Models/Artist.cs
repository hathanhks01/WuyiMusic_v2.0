using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public string Name { get; set; }

        public string Bio { get; set; } 

        public string ArtistImage { get; set; } 

        [Required] 
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Album> Albums { get; set; }
        public virtual ICollection<Track> Tracks { get; set; }
    }
}
