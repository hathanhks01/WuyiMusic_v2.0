using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.DTOS
{
    public class CommentDto
    {
        public Guid CommentId { get; set; }
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public Guid TrackId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
