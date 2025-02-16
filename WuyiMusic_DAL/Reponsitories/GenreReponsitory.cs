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
    public class GenreReponsitory : IGenreRepository
    {
        private readonly WuyiMusic_DbContext _context;
        public GenreReponsitory(WuyiMusic_DbContext context)
        {
            _context=context;
        }
      
        public async Task<Genre> AddGenre( Genre genre)
        {
            if (genre == null)
            {
                throw new ArgumentNullException(nameof(genre), "Genre cannot be null");
            }
            _context.Genres.Add(genre);
             await _context.SaveChangesAsync();
            return genre;
        }

        public async Task DeleteGenre(Guid id)
        {
            var genreToDelete = await _context.Genres.FindAsync(id);

            if (genreToDelete != null)
            {
                _context.Genres.Remove(genreToDelete);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Genre not found");
            }
        }

        public async Task<IEnumerable<Genre>> GetAllGenres()
        {
            return await _context.Genres.ToListAsync();
        }
    }
}
