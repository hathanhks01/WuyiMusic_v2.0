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
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly WuyiMusic_DbContext _context;

        public PlaylistRepository(WuyiMusic_DbContext context)
        {
            _context = context;
        }

        public async Task<Playlist> AddPlaylist(PlaylistDto playlistDto)
        {
            var playlist = new Playlist
            {
                PlaylistId = Guid.NewGuid(),
                UserId = playlistDto.UserId,
                Title = playlistDto.Tittle,
                CreatedAt = DateTime.Now,
            };
            await _context.Playlists.AddAsync(playlist);
            _context.SaveChanges();
            return playlist;
        }

        public async Task DeletePlaylist(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetAllPlaylist()
        {
            var result = await _context.Playlists
        .Include(pl => pl.User)
        .Select(pl => new
        {
            pl.PlaylistId,
            pl.Title,
            pl.CreatedAt,           
            User = pl.User == null ? null : new
            {
                pl.User.UserId,
                pl.User.Username,
                pl.User.Email,
            }
        }).ToListAsync();
            return result;
        }

        public async Task<object> GetByIdPlaylist(Guid id)
        {
            var result = await _context.Playlists.Where(pl => pl.PlaylistId == id).Select(pl => new
            {
                pl.PlaylistId,
                pl.Title,
                pl.CreatedAt,               
                User = pl.User == null ? null : new
                {
                    pl.User.UserId,
                    pl.User.Username,
                    pl.User.Email
                }
            }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<Playlist> UpdatePlaylist(PlaylistDto playlistDto)
        {
            if (playlistDto == null) throw new ArgumentNullException(nameof(playlistDto));

            var existingPlaylist = await _context.Playlists
                .FirstOrDefaultAsync(pl => pl.PlaylistId == playlistDto.PlaylistId);

            if (existingPlaylist == null) throw new InvalidOperationException("Playlist không tồn tại.");

            existingPlaylist.UserId = playlistDto.UserId;
            existingPlaylist.Title = playlistDto.Tittle;
            existingPlaylist.CreatedAt = playlistDto.CreatedAt;
            await _context.SaveChangesAsync();
            return existingPlaylist;
        }
    }

}
