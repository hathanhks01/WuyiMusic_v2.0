using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.Models;
using Microsoft.EntityFrameworkCore;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models.WuyiMusic_DAL.Models;

namespace WuyiMusic_DAL.Reponsitories
{
    public class FavoriteTrackRepository : IFavoriteTrackRepository
    {
        private readonly WuyiMusic_DbContext _dbContext;

        public FavoriteTrackRepository(WuyiMusic_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Thêm track vào danh sách yêu thích
        public async Task AddFavoriteTrack(Guid userId, Guid trackId)
        {
            var favoriteTrack = new UserFavoriteTrack
            {
                UserId = userId,
                TrackId = trackId
            };
            await _dbContext.UserFavoriteTracks.AddAsync(favoriteTrack);
            await _dbContext.SaveChangesAsync();
        }

        // Xóa track khỏi danh sách yêu thích
        public async Task RemoveFavoriteTrack(Guid userId, Guid trackId)
        {
            var favoriteTrack = await _dbContext.UserFavoriteTracks
                .FirstOrDefaultAsync(uft => uft.UserId == userId && uft.TrackId == trackId);

            if (favoriteTrack != null)
            {
                _dbContext.UserFavoriteTracks.Remove(favoriteTrack);
                await _dbContext.SaveChangesAsync();
            }
        }

        // Kiểm tra xem track đã được yêu thích hay chưa
        public async Task<bool> IsTrackFavorited(Guid userId, Guid trackId)
        {
            return await _dbContext.UserFavoriteTracks
                .AnyAsync(uft => uft.UserId == userId && uft.TrackId == trackId);
        }

    
    }
}
