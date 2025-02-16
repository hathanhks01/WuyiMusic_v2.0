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
        Task<Queue> CreateQueue(Queue queue);
        Task<Queue> GetQueueWithCurrentTrack(Guid userId);
        Task<Queue> GetQueueWithItems(Guid userId);
        Task<List<PlayHistory>> GetPlayHistory(Guid userId, DateTime startDate, DateTime endDate);
        Task<List<Track>> GetPopularTracksByGenres(List<Guid> genreIds, int limit);
        Task<List<Guid>> GetFavoriteGenres(Guid userId, int limit);
        Task AddQueueItem(QueueItem queueItem);
        Task UpdateQueue(Queue queue);
        Task SavePlayHistory(PlayHistory history);
        Task<Queue> CreateRandomQueue(Guid userId, Guid initialTrackId, int numberOfTracks = 50);
    }
}
