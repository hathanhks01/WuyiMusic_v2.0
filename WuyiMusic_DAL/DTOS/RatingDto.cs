using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.DTOS
{
    public class RatingDto
    {
        public Guid RatingId { get; set; }
        public Guid UserId { get; set; }
        public Guid TrackId { get; set; }
        public int Score { get; set; }
    }
}
