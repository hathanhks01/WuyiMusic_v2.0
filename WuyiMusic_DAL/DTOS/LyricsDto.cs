using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.DTOS
{
    public class LyricsDto
    {
        public Guid LyricsId { get; set; }
        public string Content { get; set; }
        public Guid TrackId { get; set; }
    }
}
