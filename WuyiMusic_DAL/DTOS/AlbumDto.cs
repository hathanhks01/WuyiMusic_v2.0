using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.DTOS
{
    public class AlbumDto
    {
        public Guid AlbumId { get; set; }
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public Guid ArtistId { get; set; }
    }
}
