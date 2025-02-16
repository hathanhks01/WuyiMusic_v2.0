using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.Models;


namespace WuyiMusic_Services.IServices
{
    public interface IGenreService
    {
        Task<IEnumerable<Genre>> GetAllGenres();
        Task<Genre> AddGenre(Genre genre);
        Task DeleteGenre(Guid id);
    }
}
