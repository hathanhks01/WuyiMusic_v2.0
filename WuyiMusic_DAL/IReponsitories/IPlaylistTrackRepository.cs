using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_DAL.IReponsitories
{
    public interface IPlaylistTrackRepository
    {
        Task<IEnumerable<PlaylistTrack>> GetAllAsync();
        Task<PlaylistTrack> GetByIdAsync(Guid id);
        Task AddAsync(PlaylistTrack playlistTrack);
        Task UpdateAsync(PlaylistTrack playlistTrack);
        Task DeleteAsync(Guid id);
    }
}
