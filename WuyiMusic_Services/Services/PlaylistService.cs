using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_Services.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IPlaylistRepository _playlistRepo;
        public PlaylistService(IPlaylistRepository playlistRepository) 
        { 
            _playlistRepo = playlistRepository;
        }

        public async Task<Playlist> AddPlaylist(PlaylistDto playlistDto)
        {
            return await _playlistRepo.AddPlaylist(playlistDto);
        }

        public Task DeletePlaylist(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetAllPlaylist()
        {
            return await _playlistRepo.GetAllPlaylist();
        }

        public async Task<object> GetByIdPlaylist(Guid id)
        {
            return await _playlistRepo.GetByIdPlaylist(id);
        }

        public async Task<Playlist> UpdatePlaylist(PlaylistDto playlistDto)
        {
            return await _playlistRepo.UpdatePlaylist(playlistDto);
        }
    }
}
