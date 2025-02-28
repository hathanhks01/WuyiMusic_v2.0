using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_Services.IServices
{
    public interface IArtistFollowerService
    {
        Task<IEnumerable<Artist>> GetFollowedArtistsAsync(Guid userId);
        Task<IEnumerable<User>> GetArtistFollowersAsync(Guid artistId);
        Task<bool> IsFollowingAsync(Guid userId, Guid artistId);
        Task<int> GetFollowerCountAsync(Guid artistId);
        Task FollowArtistAsync(Guid userId, Guid artistId);
        Task UnfollowArtistAsync(Guid userId, Guid artistId);
    }
}
