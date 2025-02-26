using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_DAL.IReponsitories
{
    public interface IQueueRepository
    {
        Task<Queue> CreateQueueAsync(Guid userId, Guid currentTrackId);
        Task<Queue> CreateQueueFromAlbumAsync(Guid userId, Guid albumId);
        Task<Queue> GetQueueByUserIdAsync(Guid userId);
    }
}
