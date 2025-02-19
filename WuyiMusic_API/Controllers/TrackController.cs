using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackController : ControllerBase
    {
        private readonly ITrackService _trackService;

        public TrackController(ITrackService trackService)
        {
            _trackService = trackService;
        }

        // GET: api/track/getAllTrack
        [HttpGet("getAllTrack")]
        public async Task<ActionResult<IEnumerable<Track>>> GetAllTracks()
        {
            var tracks = await _trackService.GetAllAsync();
            return Ok(tracks);
        }

        // GET: api/track/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Track>> GetTrackById(Guid id)
        {
            var track = await _trackService.GetByIdAsync(id);
            if (track == null)
            {
                return NotFound();
            }
            return Ok(track);
        }

        // GET: api/track/Favorite?userId={userId}
        [HttpGet("Favorite")]
        public async Task<ActionResult<IEnumerable<Track>>> GetFavoriteTracks(Guid userId)
        {
            var tracks = await _trackService.GetFavoriteTracksAsync(userId);

            if (tracks == null || !tracks.Any())
            {
                return NotFound();
            }

            return Ok(tracks);
        }

        // POST: api/track/addtrack
        [HttpPost("addtrack")]
        [Authorize]
        public async Task<IActionResult> AddTrack([FromForm] TrackDto trackDto)
        {
            if (trackDto.File == null || trackDto.File.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                var track = new Track
                {
                    TrackId = Guid.NewGuid(),
                    Title = trackDto.Title,
                    TrackImage = trackDto.TrackImage,
                    AlbumId = trackDto.AlbumId,
                    ArtistId = trackDto.ArtistId,
                    GenreId=trackDto.GenreId,
                    FilePath = trackDto.File.FileName,
                    Likes = 0,
                };

                await _trackService.AddTrackAsync(track, trackDto.File, trackDto.ImageFile);
                return CreatedAtAction(nameof(GetTrackById), new { id = track.TrackId }, track);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateTrack/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateTrack(
     Guid id,
     [FromForm] TrackDto trackDto) // Chỉ sử dụng 1 parameter [FromForm]
        {
            try
            {
                var track = new Track
                {
                    TrackId = id,
                    Title = trackDto.Title,
                    TrackImage = trackDto.TrackImage,
                    AlbumId = trackDto.AlbumId ?? null,
                    ArtistId = trackDto.ArtistId ?? null,
                    FilePath = trackDto.File?.FileName
                };

                await _trackService.UpdateAsync(track, trackDto.File, trackDto.ImageFile);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }


        // DELETE: api/track/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrack(Guid id)
        {
            await _trackService.DeleteAsync(id);
            return NoContent();
        }

        // GET: api/track/ranking?startDate=...&endDate=...
        [HttpGet("ranking")]
        public async Task<IActionResult> GetTrackRanking([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var rankings = await _trackService.GetTrackRankingByListenCount(startDate, endDate);
            return Ok(rankings);
        }

        // GET: api/track/searchTerm?searchTerm=...
        [HttpGet]
        [Route("searchTerm")]
        public async Task<IActionResult> SearchAsync([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(new { message = "Search term cannot be empty." });
            }

            try
            {
                var result = await _trackService.SearchAsync(searchTerm);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet("GetRandom6Track/{artistId}")]
        public async Task<IEnumerable<Track>> GetRandom6TracksAsync(Guid artistId)

        {
            return await _trackService.GetRandom6Track(artistId);
        }
    }
}
