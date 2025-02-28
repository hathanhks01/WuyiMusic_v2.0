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
    public class PlaylistTrackService : IPlaylistTrackService
    {
        private readonly IPlaylistTrackRepository _playlistTrackRepo;
        public PlaylistTrackService(IPlaylistTrackRepository playlistTrackRepo)
        {
            _playlistTrackRepo = playlistTrackRepo;
        }

        public async Task<PlaylistTrack> AddPlaylistTrack(PlaylistTrackDto playlistTrackDto)
        {
            return await _playlistTrackRepo.AddPlaylistTrack(playlistTrackDto);
        }

        public Task DeletePlaylistTrack(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetAllPlaylistTrack()
        {
            return await _playlistTrackRepo.GetAllPlaylistTrack();
        }

        public async Task<object> GetByIdPlaylistTrack(Guid id)
        {
            return await _playlistTrackRepo.GetByIdPlaylistTrack(id);
        }

        public async Task<PlaylistTrack> UpdatePlaylistTrack(PlaylistTrackDto playlistTrackDto)
        {
            return await _playlistTrackRepo.UpdatePlaylistTrack(playlistTrackDto);
        }
    }
}
