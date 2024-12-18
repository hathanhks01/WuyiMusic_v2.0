using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.Models
{
    public class Comment
    {
        [Key]
        public Guid CommentId { get; set; } = Guid.NewGuid();

        [ForeignKey("Track")]
        [Required] 
        public Guid TrackId { get; set; }
        public virtual Track Track { get; set; }

        [ForeignKey("User")]
        [Required] 
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        [Required] 
        public string Content { get; set; }

        [Required] 
        public DateTime CreatedAt { get; set; }
    }
}
