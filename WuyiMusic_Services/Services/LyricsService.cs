using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_Services.Services
{
    public class LyricsService : ILyricsService
    {
        private readonly ILyricsRepository _lyricsRepo;
        public LyricsService(ILyricsRepository lyricsRepo)
        {
           _lyricsRepo = lyricsRepo;
        }
        public async Task<Lyrics> AddLyrics(LyricsDto lyricsDto)
        {
            return await _lyricsRepo.AddLyrics(lyricsDto);
        }

        public Task DeleteLyrics(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetAllLyrics()
        {
           return await _lyricsRepo.GetAllLyrics();
        }

        public async Task<object> GetByIdLyrics(Guid id)
        {
            return await _lyricsRepo.GetByIdLyrics(id);
        }

        public async Task<Lyrics> UpdateLyrics(LyricsDto lyricsDto)
        {
            return await _lyricsRepo.UpdateLyrics(lyricsDto);
        }
    }
}
