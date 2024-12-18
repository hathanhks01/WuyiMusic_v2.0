using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.DTOS
{
    public class LyricsDto
    {
        public int LyricsId { get; set; }
        public string Content { get; set; }
        public int TrackId { get; set; }
    }
}
