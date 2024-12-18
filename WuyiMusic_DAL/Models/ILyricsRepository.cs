using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.Models
{
    public interface ILyricsRepository
    {
        Task<IEnumerable<Lyrics>> GetAllAsync();
        Task<Lyrics> GetByIdAsync(Guid id);
        Task AddAsync(Lyrics lyrics);
        Task UpdateAsync(Lyrics lyrics);
        Task DeleteAsync(Guid id);
    }
}
