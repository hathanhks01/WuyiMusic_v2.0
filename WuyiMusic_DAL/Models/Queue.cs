using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.Models
{
    public class Queue
    {
        [Key]
        public Guid QueueId { get; set; } = Guid.NewGuid();

        [Required]
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public Guid? CurrentTrackId { get; set; }
        [ForeignKey("CurrentTrackId")]
        public virtual Track CurrentTrack { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsShuffled { get; set; } = false;
        public bool IsRepeated { get; set; } = false;

        public virtual ICollection<QueueItem> QueueItems { get; set; }
    }
}
