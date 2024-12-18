using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_DAL.IReponsitories
{
    public interface IAlbumRepository
    {
        Task<IEnumerable<Album>> GetAllAsync();
        Task<Album> GetByIdAsync(Guid id);
        Task AddAsync(Album album);
        Task UpdateAsync(Album album);
        Task DeleteAsync(Guid id);
    }
}
