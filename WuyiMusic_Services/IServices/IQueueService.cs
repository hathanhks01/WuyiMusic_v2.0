using WuyiMusic_DAL.Models;

public interface IQueueService
{
    Task<Queue> CreateUserQueue(Guid userId);
    Task<Track> GetCurrentTrack(Guid userId);
    Task ReorderQueue(Guid userId, Guid trackId, int newPosition);
    Task ToggleShuffle(Guid userId);
    Task ToggleRepeat(Guid userId);
    Task<PlayHistory> AddToHistory(Guid userId, Guid trackId, TimeSpan playDuration);
    Task<TimeSpan> GetListeningTime(Guid userId, DateTime startDate, DateTime endDate);
    Task<List<Track>> GetRecommendations(Guid userId);
    Task AddToQueue(Guid userId, Guid trackId);
    Task PlayNextAsync(Guid userId);
    Task TrackFinishedPlaying(Guid userId, Guid trackId, TimeSpan actualPlayDuration);
    Task RemoveFromQueue(Guid userId, Guid trackId);
    Task ClearQueue(Guid userId);
    Task<Queue> GetQueueWithItems(Guid userId);
    Task<Queue> CreateRandomQueue(Guid userId, Guid initialTrackId, int numberOfTracks = 50);
}