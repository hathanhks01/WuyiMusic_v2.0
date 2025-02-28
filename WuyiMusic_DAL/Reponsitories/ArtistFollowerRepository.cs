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
    public class ArtistFollowerRepository : IArtistFollowerRepository
    {
        private readonly WuyiMusic_DbContext _context;

        public ArtistFollowerRepository(WuyiMusic_DbContext context)
        {
            _context = context;
        }

        public async Task<ArtistFollower> GetFollowerAsync(Guid userId, Guid artistId)
        {
            return await _context.ArtistFollowers
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ArtistId == artistId);
        }

        public async Task<IEnumerable<Artist>> GetFollowedArtistsAsync(Guid userId)
        {
            return await _context.ArtistFollowers
                .Where(f => f.UserId == userId)
                .Select(f => f.Artist)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetArtistFollowersAsync(Guid artistId)
        {
            return await _context.ArtistFollowers
                .Where(f => f.ArtistId == artistId)
                .Select(f => f.User)
                .ToListAsync();
        }

        public async Task<bool> IsFollowingAsync(Guid userId, Guid artistId)
        {
            return await _context.ArtistFollowers
                .AnyAsync(f => f.UserId == userId && f.ArtistId == artistId);
        }

        public async Task<int> GetFollowerCountAsync(Guid artistId)
        {
            return await _context.ArtistFollowers
                .CountAsync(f => f.ArtistId == artistId);
        }

        public async Task AddFollowerAsync(ArtistFollower follower)
        {
            await _context.ArtistFollowers.AddAsync(follower);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFollowerAsync(Guid userId, Guid artistId)
        {
            var follower = await GetFollowerAsync(userId, artistId);
            if (follower != null)
            {
                _context.ArtistFollowers.Remove(follower);
                await _context.SaveChangesAsync();
            }
        }
    }
}
