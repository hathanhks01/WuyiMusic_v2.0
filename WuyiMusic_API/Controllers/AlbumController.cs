using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;
using WuyiMusic_Services.IServices;

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

        [HttpPost]
        public async Task<ActionResult<Album>> CreateAlbum(AlbumDto albumDto)
        {
            await _albumSer.AddAlbum(albumDto);
            return CreatedAtAction(nameof(GetAlbumById), new { id = albumDto.AlbumId }, albumDto);
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
