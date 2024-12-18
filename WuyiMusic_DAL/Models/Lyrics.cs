using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.Models
{
    public class Lyrics
    {
        [Key]
        public Guid LyricsId { get; set; } = new Guid();

        [ForeignKey("Track")]
        [Required] 
        public Guid TrackId { get; set; }
        public virtual Track Track { get; set; }

        public string Content { get; set; } 
    }
}
