using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_DAL.DTOS
{
    public class CreateAlbumRequest
    {
        public string Title { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public Guid? ArtistId { get; set; }
        public IFormFile ImageFile { get; set; }
        public List<Track> Tracks { get; set; }
        public List<IFormFile> TrackFiles { get; set; }
    }
}
