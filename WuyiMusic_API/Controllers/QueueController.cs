using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

    [HttpGet("CreateQueue")]
    public async Task<IActionResult> CreateQueueAsync(Guid userId, Guid currentTrackId)
    {
        var result = await _queueService.CreateQueueAsync(userId, currentTrackId);

        if (result == null)
        {
            return NotFound(); 
        }

        return Ok(result);
    }
    [HttpGet("CreateQueueFromAlbumAsync")]
    public async Task<IActionResult> CreateQueueFromAlbumAsync(Guid userId, Guid albumId)
    {
        var result = await _queueService.CreateQueueFromAlbumAsync(userId, albumId);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
    [HttpGet("getQueueByUserId")]
    public async Task<IActionResult> GetQueueByUserIdAsync(Guid userId)
    {
        var result = await _queueService.GetQueueByUserIdAsync(userId);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}


