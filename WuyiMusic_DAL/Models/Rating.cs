using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.Models
{
    public class Rating
    {
        [Key]
        public Guid RatingId { get; set; }= Guid.NewGuid();

        [ForeignKey("Track")]
        [Required] 
        public Guid TrackId { get; set; }
        public virtual Track Track { get; set; }

        [ForeignKey("User")]
        [Required] 
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        public int Score { get; set; }
    }
}
