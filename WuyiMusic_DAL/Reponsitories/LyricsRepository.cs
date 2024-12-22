using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_DAL.Reponsitories
{
    public class LyricsRepository : ILyricsRepository
    {
        private readonly WuyiMusic_DbContext _context;

        public LyricsRepository(WuyiMusic_DbContext context)
        {
            _context = context;
        }

        public Task<Lyrics> AddLyrics(LyricsDto lyricsDto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteLyrics(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<object>> GetAllLyrics()
        {
            throw new NotImplementedException();
        }

        public Task<object> GetByIdLyrics(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Lyrics> UpdateLyrics(LyricsDto lyricsDto)
        {
            throw new NotImplementedException();
        }
    }

}
