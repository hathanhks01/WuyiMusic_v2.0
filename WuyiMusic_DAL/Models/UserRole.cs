using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.Models
{
    public class UserRole
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("User")]
        [Required]
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("Role")]
        [Required]
        public Guid RoleId { get; set; }
        public virtual Role Role { get; set; }
    }
}
