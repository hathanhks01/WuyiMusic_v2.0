using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_DAL.IReponsitories
{
    public interface IFavoriteTrackRepository
    {
        Task AddFavoriteTrack(Guid userId, Guid trackId);
        Task RemoveFavoriteTrack(Guid userId, Guid trackId);
        Task<bool> IsTrackFavorited(Guid userId, Guid trackId);
    }
}
