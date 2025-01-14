using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_DAL.Reponsitories
{
    public class QueueRepository : IQueueRepository
    {
        private readonly WuyiMusic_DbContext _context;

        public QueueRepository(WuyiMusic_DbContext context)
        {
            _context = context;
        }

        public async Task<Queue> CreateQueue(Queue queue)
        {
            _context.Queues.Add(queue);
            await _context.SaveChangesAsync();
            return queue;
        }

        public async Task<Queue> GetQueueWithCurrentTrack(Guid userId)
        {
            return await _context.Queues
                .Include(q => q.CurrentTrack)
                .FirstOrDefaultAsync(q => q.UserId == userId);
        }

        public async Task<Queue> GetQueueWithItems(Guid userId)
        {
            return await _context.Queues
                .Include(q => q.QueueItems)
                .FirstOrDefaultAsync(q => q.UserId == userId);
        }

        public async Task<List<PlayHistory>> GetPlayHistory(Guid userId, DateTime startDate, DateTime endDate)
        {
            return await _context.PlayHistories
                .Where(h => h.UserId == userId &&
                           h.PlayedAt >= startDate &&
                           h.PlayedAt <= endDate)
                .ToListAsync();
        }

        public async Task<List<Track>> GetPopularTracksByGenres(List<Guid> genreIds, int limit)
        {
            return await _context.Tracks
                .Include(t => t.Album)
                .Where(t => genreIds.Contains(t.Album.GenreId))
                .OrderByDescending(t => t.Likes)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<Guid>> GetFavoriteGenres(Guid userId, int limit)
        {
            return await _context.PlayHistories
                .Where(h => h.UserId == userId && h.IsCompleted)
                .Include(h => h.Track)
                .ThenInclude(t => t.Album)
                .GroupBy(h => h.Track.Album.GenreId)
                .OrderByDescending(g => g.Count())
                .Take(limit)
                .Select(g => g.Key)
                .ToListAsync();
        }

        public async Task AddQueueItem(QueueItem queueItem)
        {
            _context.QueueItems.Add(queueItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQueue(Queue queue)
        {
            _context.Queues.Update(queue);
            await _context.SaveChangesAsync();
        }

        public async Task SavePlayHistory(PlayHistory history)
        {
            _context.PlayHistories.Add(history);
            await _context.SaveChangesAsync();
        }
    }
}
