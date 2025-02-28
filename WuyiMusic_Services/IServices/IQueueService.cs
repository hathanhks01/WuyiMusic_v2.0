using WuyiMusic_DAL.Models;

public interface IQueueService
{
    Task<Queue> CreateQueueAsync(Guid userId, Guid currentTrackId);
    Task<Queue> CreateQueueFromAlbumAsync(Guid userId, Guid albumId);
    Task<Queue> GetQueueByUserIdAsync(Guid userId);
}