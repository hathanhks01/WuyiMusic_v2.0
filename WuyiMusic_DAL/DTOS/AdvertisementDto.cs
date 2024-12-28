using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.DTOS
{
    public class AdvertisementDto
    {
        public Guid AdvertisementId { get; set; }
        public string Content { get; set; }
        public Guid UserId { get; set; }
    }
}
