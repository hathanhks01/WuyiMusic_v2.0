using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.DTOS
{
    public class RatingDto
    {
        public int RatingId { get; set; }
        public int Value { get; set; }
        public int UserId { get; set; }
        public int TrackId { get; set; }
    }
}
