using Microsoft.EntityFrameworkCore;
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

        public async Task<Queue> CreateRandomQueue(Guid userId, Guid initialTrackId, int numberOfTracks = 50)
        {
            // Tạo queue mới
            var queue = new Queue
            {
                UserId = userId,
                CurrentTrackId = initialTrackId,
                IsShuffled = true,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Set<Queue>().AddAsync(queue);

            // Lấy danh sách track ngẫu nhiên, loại trừ track hiện tại
            var randomTracks = await _context.Set<Track>()
                .Where(t => t.TrackId != initialTrackId)
                .OrderBy(r => Guid.NewGuid()) // Sắp xếp ngẫu nhiên
                .Take(numberOfTracks - 1)
                .ToListAsync();

            // Thêm track hiện tại vào vị trí đầu tiên
            var queueItems = new List<QueueItem>
        {
            new QueueItem
            {
                QueueId = queue.QueueId,
                TrackId = initialTrackId,
                Position = 0,
                OriginalPosition = 0,
                AddedAt = DateTime.UtcNow
            }
        };

            // Thêm các track ngẫu nhiên vào queue
            for (int i = 0; i < randomTracks.Count; i++)
            {
                queueItems.Add(new QueueItem
                {
                    QueueId = queue.QueueId,
                    TrackId = randomTracks[i].TrackId,
                    Position = i + 1,
                    OriginalPosition = i + 1,
                    AddedAt = DateTime.UtcNow
                });
            }

            await _context.Set<QueueItem>().AddRangeAsync(queueItems);
            await _context.SaveChangesAsync();

            return queue;
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
                    .ThenInclude(t => t.TrackGenres)
                        .ThenInclude(tg => tg.Genre)
                .FirstOrDefaultAsync(q => q.UserId == userId);
        }

        public async Task<Queue> GetQueueWithItems(Guid userId)
        {
            return await _context.Queues
                .Include(q => q.QueueItems)
                    .ThenInclude(qi => qi.Track)
                        .ThenInclude(t => t.TrackGenres)
                            .ThenInclude(tg => tg.Genre)
                .FirstOrDefaultAsync(q => q.UserId == userId);
        }

        public async Task<List<PlayHistory>> GetPlayHistory(Guid userId, DateTime startDate, DateTime endDate)
        {
            return await _context.PlayHistories
                .Include(h => h.Track)
                    .ThenInclude(t => t.TrackGenres)
                        .ThenInclude(tg => tg.Genre)
                .Where(h => h.UserId == userId &&
                           h.PlayedAt >= startDate &&
                           h.PlayedAt <= endDate)
                .ToListAsync();
        }

        public async Task<List<Track>> GetPopularTracksByGenres(List<Guid> genreIds, int limit)
        {
            return await _context.Tracks
                .Include(t => t.TrackGenres)
                    .ThenInclude(tg => tg.Genre)
                .Include(t => t.Album)
                .Include(t => t.Artist)
                .Where(t => t.TrackGenres.Any(tg => genreIds.Contains(tg.GenreId)))
                .OrderByDescending(t => t.Likes)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<Guid>> GetFavoriteGenres(Guid userId, int limit)
        {
            var playedTracks = await _context.PlayHistories
                .Where(h => h.UserId == userId && h.IsCompleted)
                .Include(h => h.Track)
                    .ThenInclude(t => t.TrackGenres)
                .SelectMany(h => h.Track.TrackGenres.Select(tg => tg.GenreId))
                .GroupBy(genreId => genreId)
                .OrderByDescending(g => g.Count())
                .Take(limit)
                .Select(g => g.Key)
                .ToListAsync();

            return playedTracks;
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