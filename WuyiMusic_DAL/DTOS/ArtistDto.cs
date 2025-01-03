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
        public string Name { get; set; }
        public string Bio { get; set; }
        public string ArtistImage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
