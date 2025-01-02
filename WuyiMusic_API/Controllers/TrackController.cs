using Microsoft.AspNetCore.Mvc;
using NAudio.Wave;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;
using WuyiMusic_Services.IServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

        // GET: api/track
        [HttpGet]
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

        // POST: api/track/addtrack
        [HttpPost("addtrack")]
        public async Task<IActionResult> AddTrack([FromForm] TrackDto trackDto)
        {
            if (trackDto.File == null || trackDto.File.Length == 0)
                return BadRequest("No file uploaded.");

            // Tạo đường dẫn tạm thời để lưu file
            var tempFilePath = Path.GetTempFileName();

            // Lưu file vào đường dẫn tạm thời
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await trackDto.File.CopyToAsync(stream);
            }

            // Tính toán thời gian từ file âm thanh
            TimeSpan duration;
            using (var reader = new AudioFileReader(tempFilePath))
            {
                duration = reader.TotalTime;
            }

            var track = new Track
            {
                TrackId = Guid.NewGuid(),
                Title = trackDto.Title,
                Duration = duration, 
                AlbumId = trackDto.AlbumId,
                ArtistId = trackDto.ArtistId,
                FilePath = trackDto.File.FileName 
            };

            await _trackService.AddTrackAsync(track, trackDto.File);

            // Xóa file tạm sau khi đã sử dụng
            System.IO.File.Delete(tempFilePath);

            return CreatedAtAction(nameof(GetTrackById), new { id = track.TrackId }, track);
        }

        // PUT: api/track/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrack(Guid id, [FromBody] Track track)
        {
            if (id != track.TrackId)
            {
                return BadRequest();
            }

            await _trackService.UpdateAsync(track);
            return NoContent();
        }

        // DELETE: api/track/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrack(Guid id)
        {
            await _trackService.DeleteAsync(id);
            return NoContent();
        }


        // GET: api/track/play/{id}
        [HttpGet("play/{id}")]
        public async Task<IActionResult> PlayTrack(Guid id)
        {
            var track = await _trackService.GetByIdAsync(id);
            if (track == null || string.IsNullOrEmpty(track.FilePath))
            {
                return NotFound();
            }

            // Đường dẫn đầy đủ đến file âm thanh
            var filePath = Path.Combine("path_to_your_audio_files", track.FilePath);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var audioFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(audioFileStream, "audio/mpeg"); // Hoặc loại MIME phù hợp với định dạng âm thanh của bạn
        }
    }
}
