using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.DTOS
{
    public class PlaylistDto
    {
        public int PlaylistId { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
    }
}
