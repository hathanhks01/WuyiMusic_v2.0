using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.DTOS
{
    public class TrackDto
    {
        public int TrackId { get; set; }
        public string Title { get; set; }
        public TimeSpan Duration { get; set; }
        public int AlbumId { get; set; }
    }
}
