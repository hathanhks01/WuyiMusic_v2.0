using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistTrackController : ControllerBase
    {
        private readonly IPlaylistTrackService _playlistTrackSer;
        public PlaylistTrackController(IPlaylistTrackService playlistTrackSer)
        {
            _playlistTrackSer = playlistTrackSer;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlaylistTrack>>> GetAllPlaylistTrack()
        {
            var playlistTrack = await _playlistTrackSer.GetAllPlaylistTrack();
            return Ok(playlistTrack);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlaylistTrack>> GetPlaylistTrackById(Guid id)
        {
            var playlistTrack = await _playlistTrackSer.GetByIdPlaylistTrack(id);
            if (playlistTrack == null)
            {
                return NotFound();
            }
            return Ok(playlistTrack);
        }

        [HttpPost]
        public async Task<ActionResult<PlaylistTrack>> CreatePlaylistTrack(PlaylistTrackDto playlistTrackDto)
        {
            await _playlistTrackSer.AddPlaylistTrack(playlistTrackDto);
            return CreatedAtAction(nameof(GetPlaylistTrackById), new { id = playlistTrackDto.Id }, playlistTrackDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlaylistTrack(Guid id, PlaylistTrackDto playlistTrackDto)
        {
            if (id != playlistTrackDto.Id)
            {
                return BadRequest();
            }

            await _playlistTrackSer.UpdatePlaylistTrack(playlistTrackDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaylistTrack(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
