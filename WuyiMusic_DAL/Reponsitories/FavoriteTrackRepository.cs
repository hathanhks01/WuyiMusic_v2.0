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
        private readonly ITrackRepository _trackRepository;

        public FavoriteTrackRepository(
            WuyiMusic_DbContext dbContext,
            ITrackRepository trackRepository)
        {
            _dbContext = dbContext;
            _trackRepository = trackRepository;
        }

        public async Task AddFavoriteTrack(Guid userId, Guid trackId)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // Kiểm tra đã favorite chưa
                    var existingFavorite = await _dbContext.UserFavoriteTracks
                        .FirstOrDefaultAsync(uft => uft.UserId == userId && uft.TrackId == trackId);

                    if (existingFavorite != null)
                        return;

                    // Thêm favorite track
                    var favoriteTrack = new UserFavoriteTrack
                    {
                        UserId = userId,
                        TrackId = trackId
                    };
                    await _dbContext.UserFavoriteTracks.AddAsync(favoriteTrack);

                    // Tăng likes
                    var track = await _trackRepository.GetByIdAsync(trackId);
                    if (track != null)
                    {
                        track.Likes = (track.Likes ?? 0) + 1;
                        await _trackRepository.UpdateAsync(track);
                    }

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task RemoveFavoriteTrack(Guid userId, Guid trackId)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // Tìm favorite track để xóa
                    var favoriteTrack = await _dbContext.UserFavoriteTracks
                        .FirstOrDefaultAsync(uft => uft.UserId == userId && uft.TrackId == trackId);

                    if (favoriteTrack == null)
                        return;

                    _dbContext.UserFavoriteTracks.Remove(favoriteTrack);

                    // Giảm likes
                    var track = await _trackRepository.GetByIdAsync(trackId);
                    if (track != null)
                    {
                        track.Likes = Math.Max((track.Likes ?? 0) - 1, 0);
                        await _trackRepository.UpdateAsync(track);
                    }

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<bool> IsTrackFavorited(Guid userId, Guid trackId)
        {
            return await _dbContext.UserFavoriteTracks
                .AnyAsync(uft => uft.UserId == userId && uft.TrackId == trackId);
        }
    }
}
