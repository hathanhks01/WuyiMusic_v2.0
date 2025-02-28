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

    public async Task<Queue> CreateQueueAsync(Guid userId, Guid currentTrackId)
    {
        return await _repository.CreateQueueAsync(userId, currentTrackId);

    }

    public async Task<Queue> CreateQueueFromAlbumAsync(Guid userId, Guid albumId)
    {
        return await _repository.CreateQueueFromAlbumAsync(userId, albumId);
    }

    public async Task<Queue> GetQueueByUserIdAsync(Guid userId)
    {
       return await _repository.GetQueueByUserIdAsync(userId);
    }
}