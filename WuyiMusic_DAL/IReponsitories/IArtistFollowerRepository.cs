using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_DAL.IReponsitories
{
    public interface IArtistFollowerRepository
    {
        Task<ArtistFollower> GetFollowerAsync(Guid userId, Guid artistId);
        Task<IEnumerable<Artist>> GetFollowedArtistsAsync(Guid userId);
        Task<IEnumerable<User>> GetArtistFollowersAsync(Guid artistId);
        Task<bool> IsFollowingAsync(Guid userId, Guid artistId);
        Task<int> GetFollowerCountAsync(Guid artistId);
        Task AddFollowerAsync(ArtistFollower follower);
        Task RemoveFollowerAsync(Guid userId, Guid artistId);
    }
}
