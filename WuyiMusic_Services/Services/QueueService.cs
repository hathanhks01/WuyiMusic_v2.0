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
        if (queue == null) return;

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
        if (queue == null) return;

        queue.IsShuffled = !queue.IsShuffled;

        if (queue.IsShuffled)
        {
            var items = queue.QueueItems.ToList();
            int n = items.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                var tempItem = items[k];
                items[k] = items[n];
                items[n] = tempItem;
            }

            for (int i = 0; i < items.Count; i++)
            {
                items[i].Position = i;
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
        var queue = await _repository.GetQueueWithItems(userId);
        if (queue == null)
        {
            queue = await CreateUserQueue(userId);
        }

        var lastPosition = queue.QueueItems.Any()
            ? queue.QueueItems.Max(qi => qi.Position)
            : -1;

        var queueItem = new QueueItem
        {
            QueueId = queue.QueueId,
            TrackId = trackId,
            Position = lastPosition + 1,
            AddedAt = DateTime.UtcNow
        };

        await _repository.AddQueueItem(queueItem);
    }
}