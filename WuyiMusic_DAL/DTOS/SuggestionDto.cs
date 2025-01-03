using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.DTOS
{
    public class SuggestionDto
    {
        public Guid SuggestionId { get; set; }
        public Guid UserId { get; set; }
        public Guid TrackId { get; set; }
    }
}
