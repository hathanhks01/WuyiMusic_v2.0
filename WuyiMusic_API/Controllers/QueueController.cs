using Microsoft.AspNetCore.Mvc;
using WuyiMusic_DAL.Models;

[ApiController]
[Route("api/[controller]")]
public class QueueController : ControllerBase
{
    private readonly IQueueService _queueService;
    private readonly ILogger<QueueController> _logger;

    public QueueController(IQueueService queueService, ILogger<QueueController> logger)
    {
        _queueService = queueService;
        _logger = logger;
    }

    /// <summary>
    /// Lấy bài hát hiện tại đang phát
    /// </summary>
    [HttpGet("current")]
    [ProducesResponseType(typeof(Track), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Track>> GetCurrentTrack()
    {
        try
        {
            var userId = GetCurrentUserId();
            var track = await _queueService.GetCurrentTrack(userId);

            if (track == null)
                return NotFound("Không có bài hát nào đang phát");

            return Ok(track);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy bài hát hiện tại");
            return StatusCode(500, "Đã có lỗi xảy ra khi lấy bài hát hiện tại");
        }
    }

    /// <summary>
    /// Thêm bài hát vào queue
    /// </summary>
    [HttpPost("add")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddToQueue([FromBody] Guid trackId)
    {
        try
        {
            if (trackId == Guid.Empty)
                return BadRequest("Track ID không hợp lệ");

            var userId = GetCurrentUserId();
            await _queueService.AddToQueue(userId, trackId);

            return Ok("Đã thêm bài hát vào queue thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi thêm bài hát vào queue");
            return StatusCode(500, "Đã có lỗi xảy ra khi thêm bài hát vào queue");
        }
    }

    /// <summary>
    /// Thay đổi vị trí bài hát trong queue
    /// </summary>
    [HttpPost("reorder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ReorderQueue([FromBody] ReorderQueueRequest request)
    {
        try
        {
            if (request.TrackId == Guid.Empty)
                return BadRequest("Track ID không hợp lệ");

            if (request.NewPosition < 0)
                return BadRequest("Vị trí mới không hợp lệ");

            var userId = GetCurrentUserId();
            await _queueService.ReorderQueue(userId, request.TrackId, request.NewPosition);

            return Ok("Đã thay đổi vị trí bài hát thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi thay đổi vị trí bài hát");
            return StatusCode(500, "Đã có lỗi xảy ra khi thay đổi vị trí bài hát");
        }
    }

    /// <summary>
    /// Bật/tắt chế độ phát ngẫu nhiên
    /// </summary>
    [HttpPost("toggle-shuffle")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> ToggleShuffle()
    {
        try
        {
            var userId = GetCurrentUserId();
            await _queueService.ToggleShuffle(userId);
            return Ok("Đã thay đổi chế độ phát ngẫu nhiên");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi thay đổi chế độ shuffle");
            return StatusCode(500, "Đã có lỗi xảy ra khi thay đổi chế độ shuffle");
        }
    }

    /// <summary>
    /// Bật/tắt chế độ lặp lại
    /// </summary>
    [HttpPost("toggle-repeat")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> ToggleRepeat()
    {
        try
        {
            var userId = GetCurrentUserId();
            await _queueService.ToggleRepeat(userId);
            return Ok("Đã thay đổi chế độ lặp lại");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi thay đổi chế độ repeat");
            return StatusCode(500, "Đã có lỗi xảy ra khi thay đổi chế độ repeat");
        }
    }

    /// <summary>
    /// Lấy đề xuất bài hát
    /// </summary>
    [HttpGet("recommendations")]
    [ProducesResponseType(typeof(List<Track>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Track>>> GetRecommendations()
    {
        try
        {
            var userId = GetCurrentUserId();
            var recommendations = await _queueService.GetRecommendations(userId);
            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy đề xuất bài hát");
            return StatusCode(500, "Đã có lỗi xảy ra khi lấy đề xuất bài hát");
        }
    }

    /// <summary>
    /// Thêm vào lịch sử nghe nhạc
    /// </summary>
    [HttpPost("history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddToHistory([FromBody] AddToHistoryRequest request)
    {
        try
        {
            if (request.TrackId == Guid.Empty)
                return BadRequest("Track ID không hợp lệ");

            if (request.PlayDuration.TotalSeconds <= 0)
                return BadRequest("Thời gian phát không hợp lệ");

            var userId = GetCurrentUserId();
            await _queueService.AddToHistory(userId, request.TrackId, request.PlayDuration);

            return Ok("Đã thêm vào lịch sử nghe nhạc");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi thêm vào lịch sử nghe nhạc");
            return StatusCode(500, "Đã có lỗi xảy ra khi thêm vào lịch sử nghe nhạc");
        }
    }

    /// <summary>
    /// Lấy thống kê thời gian nghe nhạc
    /// </summary>
    [HttpGet("listening-time")]
    [ProducesResponseType(typeof(TimeSpan), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TimeSpan>> GetListeningTime([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        try
        {
            if (startDate > endDate)
                return BadRequest("Ngày bắt đầu phải nhỏ hơn ngày kết thúc");

            var userId = GetCurrentUserId();
            var listeningTime = await _queueService.GetListeningTime(userId, startDate, endDate);
            return Ok(listeningTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy thống kê thời gian nghe nhạc");
            return StatusCode(500, "Đã có lỗi xảy ra khi lấy thống kê thời gian nghe nhạc");
        }
    }

    private Guid GetCurrentUserId()
    {
        // Implement lấy user ID từ token JWT hoặc session
        // Đây chỉ là ví dụ, bạn cần implement theo hệ thống auth của bạn
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            throw new UnauthorizedAccessException("Không tìm thấy thông tin người dùng");
        }
        return userId;
    }
}

public class ReorderQueueRequest
{
    public Guid TrackId { get; set; }
    public int NewPosition { get; set; }
}

public class AddToHistoryRequest
{
    public Guid TrackId { get; set; }
    public TimeSpan PlayDuration { get; set; }
}