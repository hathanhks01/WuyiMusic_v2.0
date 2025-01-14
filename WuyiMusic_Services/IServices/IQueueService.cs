using WuyiMusic_DAL.Models;

public interface IQueueService
{
    /// <summary>
    /// Tạo một queue mới cho người dùng
    /// </summary>
    /// <param name="userId">ID của người dùng</param>
    /// <returns>Queue đã được tạo</returns>
    Task<Queue> CreateUserQueue(Guid userId);

    /// <summary>
    /// Lấy thông tin bài hát đang phát của người dùng
    /// </summary>
    /// <param name="userId">ID của người dùng</param>
    /// <returns>Thông tin bài hát hiện tại, null nếu không có</returns>
    Task<Track> GetCurrentTrack(Guid userId);

    /// <summary>
    /// Thêm một bài hát vào queue của người dùng
    /// </summary>
    /// <param name="userId">ID của người dùng</param>
    /// <param name="trackId">ID của bài hát</param>
    Task AddToQueue(Guid userId, Guid trackId);

    /// <summary>
    /// Thay đổi vị trí của bài hát trong queue
    /// </summary>
    /// <param name="userId">ID của người dùng</param>
    /// <param name="trackId">ID của bài hát cần di chuyển</param>
    /// <param name="newPosition">Vị trí mới</param>
    Task ReorderQueue(Guid userId, Guid trackId, int newPosition);

    /// <summary>
    /// Bật/tắt chế độ phát ngẫu nhiên
    /// </summary>
    /// <param name="userId">ID của người dùng</param>
    Task ToggleShuffle(Guid userId);

    /// <summary>
    /// Bật/tắt chế độ lặp lại
    /// </summary>
    /// <param name="userId">ID của người dùng</param>
    Task ToggleRepeat(Guid userId);

    /// <summary>
    /// Thêm một bản ghi vào lịch sử nghe nhạc
    /// </summary>
    /// <param name="userId">ID của người dùng</param>
    /// <param name="trackId">ID của bài hát</param>
    /// <param name="playDuration">Thời gian đã nghe</param>
    /// <returns>Thông tin lịch sử phát nhạc đã được tạo</returns>
    Task<PlayHistory> AddToHistory(Guid userId, Guid trackId, TimeSpan playDuration);

    /// <summary>
    /// Lấy danh sách các bài hát được đề xuất cho người dùng
    /// dựa trên lịch sử nghe nhạc
    /// </summary>
    /// <param name="userId">ID của người dùng</param>
    /// <returns>Danh sách bài hát được đề xuất</returns>
    Task<List<Track>> GetRecommendations(Guid userId);

    /// <summary>
    /// Tính tổng thời gian nghe nhạc của người dùng trong khoảng thời gian
    /// </summary>
    /// <param name="userId">ID của người dùng</param>
    /// <param name="startDate">Ngày bắt đầu</param>
    /// <param name="endDate">Ngày kết thúc</param>
    /// <returns>Tổng thời gian đã nghe nhạc</returns>
    Task<TimeSpan> GetListeningTime(Guid userId, DateTime startDate, DateTime endDate);
}