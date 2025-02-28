using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WuyiMusic_BLL.Services;

namespace WuyiMusic_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteTracksController : ControllerBase
    {
        private readonly IFavoriteTrackService _service;

        public FavoriteTracksController(IFavoriteTrackService service)
        {
            _service = service;
        }

        // Thêm bài hát vào danh sách yêu thích
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddFavoriteTrack(Guid userId, Guid trackId)
        {
            var result = await _service.AddFavoriteTrackAsync(userId, trackId);
            if (result)
            {
                return Ok(new { message = "Track added to favorites." });
            }
            return BadRequest(new { message = "Failed to add track to favorites." });
        }

        // Xóa bài hát khỏi danh sách yêu thích
        [HttpDelete]
        [Route("remove")]
        public async Task<IActionResult> RemoveFavoriteTrack(Guid userId, Guid trackId)
        {
            var result = await _service.RemoveFavoriteTrackAsync(userId, trackId);
            if (result)
            {
                return Ok(new { message = "Track removed from favorites." });
            }
            return BadRequest(new { message = "Failed to remove track from favorites." });
        }

        // Kiểm tra xem bài hát đã được yêu thích hay chưa
        [HttpGet]
        [Route("is-favorited")]
        public async Task<IActionResult> IsTrackFavorited(Guid userId, Guid trackId)
        {
            var isFavorited = await _service.IsTrackFavoritedAsync(userId, trackId);
            return Ok(new { isFavorited });
        }
    }
}
