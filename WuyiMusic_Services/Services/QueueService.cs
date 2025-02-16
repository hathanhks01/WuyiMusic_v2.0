using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models;

public class QueueService : IQueueService
{
    private readonly IQueueRepository _repository;
    private readonly Random _random = new Random();

    public QueueService(IQueueRepository repository)
    {
        _repository = repository;
    }

    public async Task<Queue> CreateUserQueue(Guid userId)
    {
        var queue = new Queue
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            IsShuffled = false,
            IsRepeated = false
        };

        return await _repository.CreateQueue(queue);
    }

    public async Task<Track> GetCurrentTrack(Guid userId)
    {
        var queue = await _repository.GetQueueWithCurrentTrack(userId);
        return queue?.CurrentTrack;
    }

    public async Task ReorderQueue(Guid userId, Guid trackId, int newPosition)
    {
        var queue = await _repository.GetQueueWithItems(userId);
        if (queue?.QueueItems == null || !queue.QueueItems.Any()) return;

        newPosition = Math.Clamp(newPosition, 0, queue.QueueItems.Count - 1);

        var item = queue.QueueItems.FirstOrDefault(qi => qi.TrackId == trackId);
        if (item == null) return;

        var oldPosition = item.Position;
        if (oldPosition < newPosition)
        {
            foreach (var qi in queue.QueueItems.Where(x => x.Position > oldPosition && x.Position <= newPosition))
            {
                qi.Position--;
            }
        }
        else
        {
            foreach (var qi in queue.QueueItems.Where(x => x.Position >= newPosition && x.Position < oldPosition))
            {
                qi.Position++;
            }
        }

        item.Position = newPosition;
        await _repository.UpdateQueue(queue);
    }

    public async Task ToggleShuffle(Guid userId)
    {
        var queue = await _repository.GetQueueWithItems(userId);
        if (queue?.QueueItems == null || !queue.QueueItems.Any()) return;

        queue.IsShuffled = !queue.IsShuffled;

        var items = queue.QueueItems.OrderBy(qi => qi.Position).ToList();

        if (queue.IsShuffled)
        {
            // Lưu lại thứ tự gốc trước khi xáo trộn
            foreach (var item in items)
            {
                item.OriginalPosition = item.Position;
            }

            // Fisher-Yates shuffle
            int n = items.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                var temp = items[k];
                items[k] = items[n];
                items[n] = temp;
            }

            // Cập nhật position mới
            for (int i = 0; i < items.Count; i++)
            {
                items[i].Position = i;
            }
        }
        else
        {
            // Khôi phục thứ tự ban đầu từ OriginalPosition
            foreach (var item in items.OrderBy(qi => qi.OriginalPosition))
            {
                item.Position = item.OriginalPosition;
            }
        }

        await _repository.UpdateQueue(queue);
    }

    // Thêm phương thức ToggleRepeat
    public async Task ToggleRepeat(Guid userId)
    {
        var queue = await _repository.GetQueueWithItems(userId);
        if (queue == null) return;

        queue.IsRepeated = !queue.IsRepeated;
        await _repository.UpdateQueue(queue);
    }

    public async Task<PlayHistory> AddToHistory(Guid userId, Guid trackId, TimeSpan playDuration)
    {
        var history = new PlayHistory
        {
            UserId = userId,
            TrackId = trackId,
            PlayedAt = DateTime.UtcNow,
            PlayDuration = playDuration,
            IsCompleted = playDuration.TotalSeconds / 60 >= 0.7
        };

        await _repository.SavePlayHistory(history);
        return history;
    }

    public async Task<TimeSpan> GetListeningTime(Guid userId, DateTime startDate, DateTime endDate)
    {
        var histories = await _repository.GetPlayHistory(userId, startDate, endDate);
        var totalSeconds = histories.Sum(h => h.PlayDuration.HasValue ? h.PlayDuration.Value.TotalSeconds : 0);
        return TimeSpan.FromSeconds(totalSeconds);
    }

    public async Task<List<Track>> GetRecommendations(Guid userId)
    {
        var favoriteGenres = await _repository.GetFavoriteGenres(userId, 3);
        return await _repository.GetPopularTracksByGenres(favoriteGenres, 10);
    }

    public async Task AddToQueue(Guid userId, Guid trackId)
    {
        var queue = await _repository.GetQueueWithItems(userId) ?? await CreateUserQueue(userId);

        queue.QueueItems ??= new List<QueueItem>();

        var newItem = new QueueItem
        {
            TrackId = trackId,
            OriginalPosition = queue.QueueItems.Count,
            AddedAt = DateTime.UtcNow
        };

        if (queue.IsShuffled)
        {
            // Chèn ngẫu nhiên vào vị trí bất kỳ
            int randomPosition = _random.Next(queue.QueueItems.Count + 1);
            newItem.Position = randomPosition;

            // Đẩy các item phía sau lùi 1 vị trí
            foreach (var item in queue.QueueItems.Where(qi => qi.Position >= randomPosition))
            {
                item.Position++;
            }
        }
        else
        {
            newItem.Position = queue.QueueItems.Count;
        }

        queue.QueueItems.Add(newItem);
        await _repository.UpdateQueue(queue);
    }
    public async Task<Track> GetNextTrackAsync(Guid userId)
    {
        var queue = await _repository.GetQueueWithItems(userId);
        if (queue?.CurrentTrackId == null) return null;

        // Repeat mode check
        if (queue.IsRepeated) return queue.CurrentTrack;

        var currentItem = queue.QueueItems.FirstOrDefault(qi => qi.TrackId == queue.CurrentTrackId);
        if (currentItem == null) return null;

        // Tìm bài tiếp theo theo logic đúng
        QueueItem nextItem = null;

        if (queue.IsShuffled)
        {
            // Tìm item có Position > currentItem.Position và nhỏ nhất
            nextItem = queue.QueueItems
                .Where(qi => qi.Position > currentItem.Position)
                .OrderBy(qi => qi.Position)
                .FirstOrDefault();
        }
        else
        {
            // Tìm theo OriginalPosition
            nextItem = queue.QueueItems
                .FirstOrDefault(qi => qi.OriginalPosition == currentItem.OriginalPosition + 1);
        }

        return nextItem?.Track;
    }

    public async Task PlayNextAsync(Guid userId)
    {
        var nextTrack = await GetNextTrackAsync(userId);
        var queue = await _repository.GetQueueWithItems(userId);

        if (nextTrack != null)
        {
            queue.CurrentTrackId = nextTrack.TrackId;
        }
        else
        {
            // Tự động tạo queue mới từ recommendation
            var recommendations = await GetRecommendations(userId);
            if (recommendations.Any())
            {
                queue.CurrentTrackId = null;
                queue.QueueItems.Clear();
                foreach (var track in recommendations)
                {
                    await AddToQueue(userId, track.TrackId);
                }
                await PlayNextAsync(userId); // Chơi bài đầu tiên
            }
        }

        await _repository.UpdateQueue(queue);
    }
    // Kiểm tra queue rỗng
    private async Task<bool> IsQueueEmpty(Guid userId)
    {
        var queue = await _repository.GetQueueWithItems(userId);
        return queue?.QueueItems?.Any() != true;
    }

    // Tự động phát khi thêm bài đầu tiên
    public async Task AddToQueueAndPlay(Guid userId, Guid trackId)
    {
        if (await IsQueueEmpty(userId))
        {
            await AddToQueue(userId, trackId);
            var queue = await _repository.GetQueueWithItems(userId);
            queue.CurrentTrackId = trackId;
            await _repository.UpdateQueue(queue);
        }
        else
        {
            await AddToQueue(userId, trackId);
        }
    }
    public async Task TrackFinishedPlaying(Guid userId, Guid trackId, TimeSpan actualPlayDuration)
    {
        await AddToHistory(userId, trackId, actualPlayDuration);
        await PlayNextAsync(userId);
    }
    public async Task RemoveFromQueue(Guid userId, Guid trackId)
    {
        var queue = await _repository.GetQueueWithItems(userId);
        if (queue?.QueueItems == null) return;

        var item = queue.QueueItems.FirstOrDefault(qi => qi.TrackId == trackId);
        if (item == null) return;

        // Cập nhật lại position cho các item phía sau
        foreach (var qi in queue.QueueItems.Where(x => x.Position > item.Position))
        {
            qi.Position--;
        }

        queue.QueueItems.Remove(item);

        // Nếu xóa bài đang phát, chuyển sang bài tiếp theo
        if (queue.CurrentTrackId == trackId)
        {
            await PlayNextAsync(userId);
        }

        await _repository.UpdateQueue(queue);
    }

    public async Task ClearQueue(Guid userId)
    {
        var queue = await _repository.GetQueueWithItems(userId);
        if (queue == null) return;

        queue.QueueItems.Clear();
        queue.CurrentTrackId = null;

        await _repository.UpdateQueue(queue);
    }
    public async Task<Queue> GetQueueWithItems(Guid userId)
    {
        var queue = await _repository.GetQueueWithItems(userId);
        if (queue == null)
        {
            // Nếu queue chưa tồn tại, tạo mới
            queue = await CreateUserQueue(userId);
        }
        return queue;
    }

    public async Task<Queue> CreateRandomQueue(Guid userId, Guid initialTrackId, int numberOfTracks = 50)
    {
       return await _repository.CreateRandomQueue(userId, initialTrackId, numberOfTracks);
    }
}