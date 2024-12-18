using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_DAL.IReponsitories
{
    public interface ITrackRepository
    {
        Task<IEnumerable<Track>> GetAllAsync();
        Task<Track> GetByIdAsync(Guid id);
        Task AddAsync(Track track);
        Task UpdateAsync(Track track);
        Task DeleteAsync(Guid id);
    }
}
