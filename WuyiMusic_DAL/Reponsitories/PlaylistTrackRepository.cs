using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_DAL.Reponsitories
{
    public class PlaylistTrackRepository : IPlaylistTrackRepository
    {
        private readonly WuyiMusic_DbContext _context;

        public PlaylistTrackRepository(WuyiMusic_DbContext context)
        {
            _context = context;
        }

        public async Task<PlaylistTrack> AddPlaylistTrack(PlaylistTrackDto playlistTrackDto)
        {
            var playlistTrack = new PlaylistTrack
            {
                Id = Guid.NewGuid(),
                PlaylistId = playlistTrackDto.PlaylistId,
                TrackId = playlistTrackDto.TrackId,
            };
            await _context.PlaylistTracks.AddAsync(playlistTrack);
            _context.SaveChanges();
            return playlistTrack;
        }

        public Task DeletePlaylistTrack(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetAllPlaylistTrack()
        {
            var result = await _context.PlaylistTracks
                .Include(plt => plt.Playlist)
                .ThenInclude(pl => pl.User)
            .Include(plt => plt.Track)
                .ThenInclude(tr => tr.Album)
            .Include(plt => plt.Track)
                .ThenInclude(tr => tr.Artist)
            
            .Select(plt => new
            {
                plt.Id,
                Playlist = plt.Playlist == null ? null : new
                {
                    plt.Playlist.PlaylistId,
                    plt.Playlist.CreatedAt,
                    User = plt.Playlist.User == null ? null : new
                    {
                        plt.Playlist.User.UserId,
                        plt.Playlist.User.Username,
                        plt.Playlist.User.Email,
                    }
                },
                Track = plt.Track == null ? null : new
                {
                    plt.Track.TrackId,
                    plt.Track.Title,
                    plt.Track.Duration,
                    plt.Track.FilePath,
                    Album = plt.Track.Album == null ? null : new
                    {
                        plt.Track.Album.AlbumId,
                        plt.Track.Album.Title
                    },
                    Artist = plt.Track.Artist == null ? null : new
                    {
                        plt.Track.Artist.ArtistId,
                        plt.Track.Artist.Name
                    }
                }         
            }).ToListAsync();
                return result;
        }

        public async Task<object> GetByIdPlaylistTrack(Guid id)
        {
            var result = await _context.PlaylistTracks.Where(plt => plt.Id == id).Select(plt => new
            {
                plt.Id,
                Playlist = plt.Playlist == null ? null : new
                {
                    plt.Playlist.PlaylistId,
                    plt.Playlist.CreatedAt,
                    User = plt.Playlist.User == null ? null : new
                    {
                        plt.Playlist.User.UserId,
                        plt.Playlist.User.Username,
                        plt.Playlist.User.Email,
                    }
                },
                Track = plt.Track == null ? null : new
                {
                    plt.Track.TrackId,
                    plt.Track.Title,
                    plt.Track.Duration,
                    plt.Track.FilePath,
                    Album = plt.Track.Album == null ? null : new
                    {
                        plt.Track.Album.AlbumId,
                        plt.Track.Album.Title
                    },
                    Artist = plt.Track.Artist == null ? null : new
                    {
                        plt.Track.Artist.ArtistId,
                        plt.Track.Artist.Name
                    }
                }
            }).FirstOrDefaultAsync(); ;
            return result;

        }

        public async Task<PlaylistTrack> UpdatePlaylistTrack(PlaylistTrackDto playlistTrackDto)
        {
            if (playlistTrackDto == null) throw new ArgumentNullException(nameof(playlistTrackDto));

            var existingPlaylistTrack = await _context.PlaylistTracks
                .FirstOrDefaultAsync(plt => plt.Id == playlistTrackDto.Id);

            if (existingPlaylistTrack == null) throw new InvalidOperationException("PlaylistTrack không tồn tại.");

            existingPlaylistTrack.TrackId = playlistTrackDto.TrackId;
            existingPlaylistTrack.PlaylistId = playlistTrackDto.PlaylistId;
            await _context.SaveChangesAsync();
            return existingPlaylistTrack;
        }
    }

}
