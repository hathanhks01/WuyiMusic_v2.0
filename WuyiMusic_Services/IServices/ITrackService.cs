using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_Services.IServices
{
    public interface ITrackService
    {
        Task<IEnumerable<Track>> GetAllAsync();
        Task<Track> GetByIdAsync(Guid id);
        Task AddTrackAsync(Track track, IFormFile file);
        Task UpdateAsync(Track track);
        Task DeleteAsync(Guid id);
    }
}
