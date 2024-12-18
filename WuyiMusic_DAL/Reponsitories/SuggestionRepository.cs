using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_DAL.Reponsitories
{
    public class SuggestionRepository : ISuggestionRepository
    {
        private readonly WuyiMusic_DbContext _context;

        public SuggestionRepository(WuyiMusic_DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Suggestion>> GetAllAsync()
        {
            return await _context.Suggestions.ToListAsync();
        }

        public async Task<Suggestion> GetByIdAsync(Guid id)
        {
            return await _context.Suggestions.FindAsync(id);
        }

        public async Task AddAsync(Suggestion suggestion)
        {
            await _context.Suggestions.AddAsync(suggestion);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Suggestion suggestion)
        {
            _context.Suggestions.Update(suggestion);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var suggestion = await GetByIdAsync(id);
            if (suggestion != null)
            {
                _context.Suggestions.Remove(suggestion);
                await _context.SaveChangesAsync();
            }
        }
    }

}
