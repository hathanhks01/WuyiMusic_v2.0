using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.Models
{
    public class QueueItem
    {
        [Key]
        public Guid QueueItemId { get; set; } = Guid.NewGuid();

        [Required]
        [ForeignKey("Queue")]
        public Guid QueueId { get; set; }
        public virtual Queue Queue { get; set; }

        [Required]
        [ForeignKey("Track")]
        public Guid TrackId { get; set; }
        public virtual Track Track { get; set; }

        [Required]
        public int Position { get; set; }  // Vị trí trong queue

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
