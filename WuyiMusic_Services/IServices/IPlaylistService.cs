using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_Services.IServices
{
    public interface IPlaylistService
    {
        Task<IEnumerable<object>> GetAllPlaylist();
        Task<object> GetByIdPlaylist(Guid id);
        Task<Playlist> AddPlaylist(PlaylistDto playlistDto);
        Task<Playlist> UpdatePlaylist(PlaylistDto playlistDto);
        Task DeletePlaylist(Guid id);
    }
}
