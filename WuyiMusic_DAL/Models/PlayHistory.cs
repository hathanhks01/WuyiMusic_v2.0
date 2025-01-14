using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.Models
{
    public class PlayHistory
    {
        [Key]
        public Guid HistoryId { get; set; } = Guid.NewGuid();

        [Required]
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        [ForeignKey("Track")]
        public Guid TrackId { get; set; }
        public virtual Track Track { get; set; }

        [Required]
        public DateTime PlayedAt { get; set; } = DateTime.UtcNow;

        public TimeSpan? PlayDuration { get; set; }  // Thời gian đã phát

        public bool IsCompleted { get; set; }  // Đã phát hết bài chưa
    }
}
