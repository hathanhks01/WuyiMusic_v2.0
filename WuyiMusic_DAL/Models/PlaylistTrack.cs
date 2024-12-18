using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.Models
{
    public class PlaylistTrack
    {
        [Key]
        public Guid Id { get; set; }= Guid.NewGuid();

        [ForeignKey("Playlist")]
        [Required] 
        public Guid PlaylistId { get; set; }
        public virtual Playlist Playlist { get; set; }

        [ForeignKey("Track")]
        [Required] 
        public Guid TrackId { get; set; }
        public virtual Track Track { get; set; }
    }
}
