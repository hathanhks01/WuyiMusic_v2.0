using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.DTOS
{
    public class PlaylistTrackDto
    {
        public Guid Id { get; set; } 
        public Guid PlaylistId { get; set; }
        public Guid TrackId { get; set; }
    }
}
