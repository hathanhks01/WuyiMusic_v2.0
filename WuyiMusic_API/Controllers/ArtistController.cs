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
    public class ArtistController : ControllerBase
    {
        private readonly IArtistService _artistSer;
        public ArtistController(IArtistService artistSer)
        {
            _artistSer = artistSer;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Artist>>> GetAllArtist()
        {
            var aritists = await _artistSer.GetAllArtist();
            return Ok(aritists);
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

        [HttpPost]
        public async Task<ActionResult<Artist>> CreateArtist(ArtistDto artistDto)
        {
            await _artistSer.AddArtist(artistDto);
            return CreatedAtAction(nameof(GetArtistById), new { id = artistDto.ArtistId }, artistDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArtist(Guid id, ArtistDto artistDto)
        {
            if (id != artistDto.ArtistId)
            {
                return BadRequest();
            }

            await _artistSer.UpdateArtist(artistDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArtist(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
