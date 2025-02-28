using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;

namespace WuyiMusic_DAL.Models
{
    public interface ILyricsRepository
    {
        Task<IEnumerable<object>> GetAllLyrics();
        Task<object> GetByIdLyrics(Guid id);
        Task<Lyrics> AddLyrics(LyricsDto lyricsDto);
        Task<Lyrics> UpdateLyrics(LyricsDto lyricsDto);
        Task DeleteLyrics(Guid id);
    }
}
