using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace WuyiMusic_DAL.Models
    {
        public class UserFavoriteTrack
        {
            [Key]
            public Guid Id { get; set; } = Guid.NewGuid();

            [ForeignKey("User")]
            public Guid UserId { get; set; }
            public virtual User User { get; set; }

            [ForeignKey("Track")]
            public Guid TrackId { get; set; }
            public virtual Track Track { get; set; }
        }
    }

}
