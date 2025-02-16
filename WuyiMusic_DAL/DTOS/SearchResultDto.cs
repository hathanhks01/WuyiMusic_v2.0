using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.DTOS
{
    public class SearchResultDto
    {
        public List<TrackDto> Tracks { get; set; }
        public List<ArtistDto> Artists { get; set; }
        public List<AlbumDto> Albums { get; set; }
        public List<PlaylistDto> Playlists { get; set; }
    }
}
