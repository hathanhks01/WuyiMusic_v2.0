using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_Services.IServices
{
    public interface ILyricsService
    {
        Task<IEnumerable<object>> GetAllLyrics();
        Task<object> GetByIdLyrics(Guid id);
        Task<Lyrics> AddLyrics(LyricsDto lyricsDto);
        Task<Lyrics> UpdateLyrics(LyricsDto lyricsDto);
        Task DeleteLyrics(Guid id);
    }
}
