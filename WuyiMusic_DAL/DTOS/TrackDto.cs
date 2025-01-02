using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.Models;
using Microsoft.AspNetCore.Http;

namespace WuyiMusic_DAL.DTOS
{
    public class TrackDto
    {
        public string Title { get; set; }
        public TimeSpan? Duration { get; set; }
        public Guid? AlbumId { get; set; }

        public Guid? ArtistId { get; set; }

        public int Likes { get; set; }
        [Required]
        public IFormFile File { get; set; }
    }
}
