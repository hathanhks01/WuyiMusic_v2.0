using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WuyiMusic_Services.IServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WuyiMusic_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistFollowerController : ControllerBase
    {
        private readonly IArtistFollowerService _service;

        public ArtistFollowerController(IArtistFollowerService service)
        {
            _service = service;
        }

        [HttpGet("followed-artists")]
        [Authorize]
        public async Task<IActionResult> GetFollowedArtists()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var artists = await _service.GetFollowedArtistsAsync(userId);
            return Ok(artists);
        }

        [HttpGet("artist/{artistId}/followers")]
        public async Task<IActionResult> GetArtistFollowers(Guid artistId)
        {
            var followers = await _service.GetArtistFollowersAsync(artistId);
            return Ok(followers);
        }

        [HttpGet("artist/{artistId}/follower-count")]
        public async Task<IActionResult> GetFollowerCount(Guid artistId)
        {
            var count = await _service.GetFollowerCountAsync(artistId);
            return Ok(count);
        }

        [HttpGet("artist/{artistId}/is-following")]
        [Authorize]
        public async Task<IActionResult> IsFollowing(Guid artistId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var isFollowing = await _service.IsFollowingAsync(userId, artistId);
            return Ok(isFollowing);
        }

        [HttpPost("artist/{artistId}/follow")]
        [Authorize]
        public async Task<IActionResult> FollowArtist(Guid artistId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _service.FollowArtistAsync(userId, artistId);
            return Ok();
        }

        [HttpPost("artist/{artistId}/unfollow")]
        [Authorize]
        public async Task<IActionResult> UnfollowArtist(Guid artistId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _service.UnfollowArtistAsync(userId, artistId);
            return Ok();
        }
    }
}
