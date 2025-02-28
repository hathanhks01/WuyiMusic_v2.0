using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;
using WuyiMusic_Services.IServices;
using WuyiMusic_Services.Services;

namespace WuyiMusic_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : ControllerBase
    {
        private readonly IArtistService _artistSer;
        public ArtistController(IArtistService artistSer)
        {
            _artistSer = artistSer;
        }
        // GET: api/artist/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<Artist>> GetArtistByUserId(Guid userId)
        {
            var artist = await _artistSer.GetArtistByUserIdAsync(userId);

            if (artist == null)
            {
                return NotFound("Artist not found for the specified user.");
            }

            return Ok(artist);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Artist>>> GetAllArtist()
        {
            var aritists = await _artistSer.GetAllArtist();
            return Ok(aritists);
        }
        [HttpGet]
        [Route("random")]
        public async Task<ActionResult<IEnumerable<Artist>>> GetRandomArtist()
        {
            var artists = await _artistSer.GetRandomArtistsAsync();
            return Ok(artists);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Artist>> GetArtistById(Guid id)
        {
            var aritists = await _artistSer.GetByIdArtist(id);
            if (aritists == null)
            {
                return NotFound();
            }
            return Ok(aritists);
        }

        [HttpPost("CreateArtist")]
        public async Task<ActionResult<Artist>> CreateArtist([FromForm] ArtistDto artistDto, [FromQuery] Guid userId)
        {
            await _artistSer.AddArtist(artistDto, userId);
            return CreatedAtAction(nameof(GetArtistById), new { id = artistDto.ArtistId }, artistDto);
        }
        [HttpPost("CreateArtistForAdm")]
        public async Task<ActionResult<Artist>> CreateArtistForAdm([FromForm] ArtistDto artistDto)
        {
            var createdArtist = await _artistSer.AddArtistForAdm(artistDto);
            return CreatedAtAction(nameof(GetArtistById), new { id = createdArtist.ArtistId }, createdArtist);
        }


        [HttpPut("UpdateArtist/{id}")]
        public async Task<IActionResult> UpdateArtist(Guid id, [FromForm] ArtistDto artistDto)
        {
            try
            {
                artistDto.ArtistId = id;
                var updatedArtist = await _artistSer.UpdateArtist(artistDto);
                return Ok(updatedArtist);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết ở đây
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArtist(Guid id)
        {
            try
            {
                await _artistSer.DeleteArtist(id);
                return Ok();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
