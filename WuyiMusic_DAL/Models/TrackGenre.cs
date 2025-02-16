using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.Models
{
    public class TrackGenre
    {
        public Guid TrackId { get; set; }
        public virtual Track Track { get; set; }

        public Guid GenreId { get; set; }
        public virtual Genre Genre { get; set; }
    }
}
