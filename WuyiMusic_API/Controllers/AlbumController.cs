using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;
using WuyiMusic_Services.IServices;
using WuyiMusic_Services.Services;

namespace WuyiMusic_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private readonly IAlbumService _albumSer;
        public AlbumController(IAlbumService albumSer)
        {
            _albumSer = albumSer;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Album>>> GetAllAlbum()
        {
            var albums = await _albumSer.GetAllAlbum();
            return Ok(albums);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Album>> GetAlbumById(Guid id)
        {
            var albums = await _albumSer.GetByIdAlbum(id);
            if (albums == null)
            {
                return NotFound();
            }
            return Ok(albums);
        }

        [Authorize]
        [RequestSizeLimit(104857600)]
        [HttpPost("addAlbum")]
        public async Task<IActionResult> CreateAlbum([FromForm] CreateAlbumRequest request)
        {
            try
            {
                if (request.ImageFile == null || request.ImageFile.Length == 0)
                {
                    return BadRequest("Vui lòng upload ảnh album");
                }
                if (request.TrackFiles == null || request.TrackFiles.Count == 0)
                {
                    return BadRequest("Vui lòng upload ít nhất một file nhạc");
                }

                // Validate matching counts
                if (request.TrackFiles.Count != request.Tracks.Count)
                {
                    return BadRequest("Số lượng file nhạc và thông tin track không khớp");
                }

                // Tạo Dictionary để map index với file nhạc
                var trackFiles = new Dictionary<int, IFormFile>();
                for (int i = 0; i < request.TrackFiles.Count; i++)
                {
                    trackFiles.Add(i, request.TrackFiles[i]);
                }

                // Tạo album object
                var album = new Album
                {
                    Title = request.Title,
                    ReleaseDate = request.ReleaseDate,
                    ArtistId = request.ArtistId,
                    Tracks = request.Tracks
                };

                var result = await _albumSer.AddAlbum(album, request.ImageFile, trackFiles);
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAlbum(Guid id, AlbumDto albumDto)
        {
            if (id != albumDto.AlbumId)
            {
                return BadRequest();
            }

            await _albumSer.UpdateAlbum(albumDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlbum(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
