using System;
using System.Threading.Tasks;
using WuyiMusic_DAL.IReponsitories;

namespace WuyiMusic_BLL.Services
{
    public class FavoriteTrackService : IFavoriteTrackService
    {
        private readonly IFavoriteTrackRepository _favoriteTrackRepository;

        public FavoriteTrackService(IFavoriteTrackRepository favoriteTrackRepository)
        {
            _favoriteTrackRepository = favoriteTrackRepository;
        }

        // Thêm track vào danh sách yêu thích
        public async Task<bool> AddFavoriteTrackAsync(Guid userId, Guid trackId)
        {
            try
            {
                await _favoriteTrackRepository.AddFavoriteTrack(userId, trackId);
                return true;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần
                Console.WriteLine($"Error adding favorite track: {ex.Message}");
                return false;
            }
        }

        // Kiểm tra xem track đã được yêu thích hay chưa
        public async Task<bool> IsTrackFavoritedAsync(Guid userId, Guid trackId)
        {
            return await _favoriteTrackRepository.IsTrackFavorited(userId, trackId);
        }

        // Xóa track khỏi danh sách yêu thích
        public async Task<bool> RemoveFavoriteTrackAsync(Guid userId, Guid trackId)
        {
            try
            {
                await _favoriteTrackRepository.RemoveFavoriteTrack(userId, trackId);
                return true;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần
                Console.WriteLine($"Error removing favorite track: {ex.Message}");
                return false;
            }
        }
    }
}
