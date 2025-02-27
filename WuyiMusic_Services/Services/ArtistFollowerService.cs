using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_Services.Services
{
    public class ArtistFollowerService : IArtistFollowerService
    {
        private readonly IArtistFollowerRepository _repository;

        public ArtistFollowerService(IArtistFollowerRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Artist>> GetFollowedArtistsAsync(Guid userId)
        {
            return await _repository.GetFollowedArtistsAsync(userId);
        }

        public async Task<IEnumerable<User>> GetArtistFollowersAsync(Guid artistId)
        {
            return await _repository.GetArtistFollowersAsync(artistId);
        }

        public async Task<bool> IsFollowingAsync(Guid userId, Guid artistId)
        {
            return await _repository.IsFollowingAsync(userId, artistId);
        }

        public async Task<int> GetFollowerCountAsync(Guid artistId)
        {
            return await _repository.GetFollowerCountAsync(artistId);
        }

        public async Task FollowArtistAsync(Guid userId, Guid artistId)
        {
            var isAlreadyFollowing = await _repository.IsFollowingAsync(userId, artistId);
            if (!isAlreadyFollowing)
            {
                var follower = new ArtistFollower
                {
                    UserId = userId,
                    ArtistId = artistId,
                    FollowedAt = DateTime.Now
                };
                await _repository.AddFollowerAsync(follower);
            }
        }

        public async Task UnfollowArtistAsync(Guid userId, Guid artistId)
        {
            await _repository.RemoveFollowerAsync(userId, artistId);
        }
    }
}
