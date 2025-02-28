using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_DAL.IReponsitories
{
    public interface IPlaylistTrackRepository
    {
        Task<IEnumerable<object>> GetAllPlaylistTrack();
        Task<object> GetByIdPlaylistTrack(Guid id);
        Task<PlaylistTrack> AddPlaylistTrack(PlaylistTrackDto playlistTrackDto);
        Task<PlaylistTrack> UpdatePlaylistTrack(PlaylistTrackDto playlistTrackDto);
        Task DeletePlaylistTrack(Guid id);
    }
}
