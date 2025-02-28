using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_Services.Services
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;
        public GenreService(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }
        public async Task<Genre> AddGenre(Genre genre)
        {
            return await _genreRepository.AddGenre(genre);
        }

        public async Task DeleteGenre(Guid id)
        {
            await _genreRepository.DeleteGenre(id);
        }

        public async Task<IEnumerable<Genre>> GetAllGenres()
        {
           return await _genreRepository.GetAllGenres();
        }
    }
}
