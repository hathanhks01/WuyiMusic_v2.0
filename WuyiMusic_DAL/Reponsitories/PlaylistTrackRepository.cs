using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<PlaylistTrack>> GetAllAsync()
        {
            return await _context.PlaylistTracks.ToListAsync();
        }

        public async Task<PlaylistTrack> GetByIdAsync(Guid id)
        {
            return await _context.PlaylistTracks.FindAsync(id);
        }

        public async Task AddAsync(PlaylistTrack playlistTrack)
        {
            await _context.PlaylistTracks.AddAsync(playlistTrack);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PlaylistTrack playlistTrack)
        {
            _context.PlaylistTracks.Update(playlistTrack);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var playlistTrack = await GetByIdAsync(id);
            if (playlistTrack != null)
            {
                _context.PlaylistTracks.Remove(playlistTrack);
                await _context.SaveChangesAsync();
            }
        }
    }

}
