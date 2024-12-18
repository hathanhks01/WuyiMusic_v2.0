using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.Models
{
    public class Suggestion
    {
        [Key]
        public Guid SuggestionId { get; set; }=new Guid();

        [ForeignKey("User")]
        [Required] 
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("Track")]
        [Required] 
        public Guid TrackId { get; set; }
        public virtual Track Track { get; set; }
    }
}
