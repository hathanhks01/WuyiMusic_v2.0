using System;
using System.Threading.Tasks;

namespace WuyiMusic_BLL.Services
{
    public interface IFavoriteTrackService
    {
        Task<bool> AddFavoriteTrackAsync(Guid userId, Guid trackId);
        Task<bool> RemoveFavoriteTrackAsync(Guid userId, Guid trackId);
        Task<bool> IsTrackFavoritedAsync(Guid userId, Guid trackId);
    }
}
