using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.DTOS
{
    public class SuggestionDto
    {
        public int SuggestionId { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
    }
}
