using Microsoft.EntityFrameworkCore;
using System.Linq;
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

        public async Task<Queue> CreateQueueAsync(Guid userId, Guid currentTrackId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    throw new Exception("User not found");
                }
                // 1. Xóa queue hiện tại nếu tồn tại
                var existingQueue = await _context.Queues
                    .Include(q => q.QueueItems)
                    .FirstOrDefaultAsync(q => q.UserId == userId);

                if (existingQueue != null)
                {
                    _context.QueueItems.RemoveRange(existingQueue.QueueItems);
                    _context.Queues.Remove(existingQueue);
                    await _context.SaveChangesAsync();
                }

                // 2. Tạo queue mới
                var newQueue = new Queue
                {
                    UserId = userId,
                    CurrentTrackId = currentTrackId,
                    CreatedAt = DateTime.UtcNow,
                    IsShuffled = false,
                    IsRepeated = false
                };
                _context.Queues.Add(newQueue);
                await _context.SaveChangesAsync();

                // 3. Thêm track được chọn vào đầu queue
                var currentTrackItem = new QueueItem
                {
                    QueueId = newQueue.QueueId,
                    TrackId = currentTrackId,
                    Position = 0,
                    OriginalPosition = 0,
                    AddedAt = DateTime.UtcNow
                };
                _context.QueueItems.Add(currentTrackItem);

                // 4. Lấy các track khác và xáo trộn
                var otherTracks = await _context.Tracks
                    .Where(t => t.TrackId != currentTrackId)
                    .OrderBy(_ => Guid.NewGuid())
                    .ToListAsync();

                // 5. Thêm các track vào queue
                var position = 1;
                foreach (var track in otherTracks)
                {
                    _context.QueueItems.Add(new QueueItem
                    {
                        QueueId = newQueue.QueueId,
                        TrackId = track.TrackId,
                        Position = position,
                        OriginalPosition = position,
                        AddedAt = DateTime.UtcNow
                    });
                    position++;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetQueueByUserIdAsync(userId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Queue> CreateQueueFromAlbumAsync(Guid userId, Guid albumId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    throw new Exception("Không tìm thấy người dùng");
                }

                var album = await _context.Albums
                    .Include(a => a.Tracks)
                    .FirstOrDefaultAsync(a => a.AlbumId == albumId);

                if (album == null || !album.Tracks.Any())
                {
                    throw new Exception("Không tìm thấy album hoặc album không có bài hát nào");
                }

                // 1. Xóa queue hiện tại nếu có
                var existingQueue = await _context.Queues
                    .Include(q => q.QueueItems)
                    .FirstOrDefaultAsync(q => q.UserId == userId);

                if (existingQueue != null)
                {
                    _context.QueueItems.RemoveRange(existingQueue.QueueItems);
                    _context.Queues.Remove(existingQueue);
                    await _context.SaveChangesAsync();
                }

                // 2. Tạo queue mới với bài hát đầu tiên của album
                var firstTrack = album.Tracks.First();
                var newQueue = new Queue
                {
                    UserId = userId,
                    CurrentTrackId = firstTrack.TrackId,
                    CreatedAt = DateTime.UtcNow,
                    IsShuffled = false,
                    IsRepeated = false
                };

                _context.Queues.Add(newQueue);
                await _context.SaveChangesAsync();

                // 3. Thêm tất cả các bài hát của album vào queue
                var position = 0;
                foreach (var track in album.Tracks)
                {
                    _context.QueueItems.Add(new QueueItem
                    {
                        QueueId = newQueue.QueueId,
                        TrackId = track.TrackId,
                        Position = position,
                        OriginalPosition = position,
                        AddedAt = DateTime.UtcNow
                    });
                    position++;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetQueueByUserIdAsync(userId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<Queue> GetQueueByUserIdAsync(Guid userId)
        {
            return await _context.Queues
        .Include(q => q.QueueItems
            .OrderBy(qi => qi.Position)) // Thêm OrderBy
        .ThenInclude(qi => qi.Track)
        .Include(q => q.CurrentTrack)
        .FirstOrDefaultAsync(q => q.UserId == userId);
        }

    }
}