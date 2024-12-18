using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.Models
{
    public class Advertisement
    {
        [Key]
        public Guid AdvertisementId { get; set; } = Guid.NewGuid();

        [Required] 
        public string Content { get; set; }

        [ForeignKey("User")]
        [Required] 
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
