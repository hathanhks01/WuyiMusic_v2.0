using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<Lyrics>> GetAllAsync()
        {
            return await _context.Lyrics.ToListAsync();
        }

        public async Task<Lyrics> GetByIdAsync(Guid id)
        {
            return await _context.Lyrics.FindAsync(id);
        }

        public async Task AddAsync(Lyrics lyrics)
        {
            await _context.Lyrics.AddAsync(lyrics);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Lyrics lyrics)
        {
            _context.Lyrics.Update(lyrics);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var lyrics = await GetByIdAsync(id);
            if (lyrics != null)
            {
                _context.Lyrics.Remove(lyrics);
                await _context.SaveChangesAsync();
            }
        }
    }

}
