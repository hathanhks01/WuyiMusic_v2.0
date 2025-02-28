using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.DTOS
{
    public class ArtistDto
    {
        public Guid ArtistId { get; set; }
        public string? Name { get; set; }
        public string? Bio { get; set; }
        public bool? IsVerified { get; set; } = false;
        public string? MetaLink { get; set; }
        public string? ArtistImage { get; set; } 
        public IFormFile? ArtistImageFile { get; set; }
    }
}
