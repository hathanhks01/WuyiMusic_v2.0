using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        private readonly IPlaylistService _playlistSer;
        public PlaylistController(IPlaylistService playlistSer) 
        { 
            _playlistSer = playlistSer;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Playlist>>> GetAllPlaylist()
        {
            var comments = await _playlistSer.GetAllPlaylist();
            return Ok(comments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Playlist>> GetPlaylistById(Guid id)
        {
            var playlist = await _playlistSer.GetByIdPlaylist(id);
            if (playlist == null)
            {
                return NotFound();
            }
            return Ok(playlist);
        }

        [HttpPost]
        public async Task<ActionResult<Playlist>> CreateComment(PlaylistDto playlistDto)
        {
            await _playlistSer.AddPlaylist(playlistDto);
            return CreatedAtAction(nameof(GetPlaylistById), new { id = playlistDto.PlaylistId }, playlistDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(Guid id, PlaylistDto playlistDto)
        {
            if (id != playlistDto.PlaylistId)
            {
                return BadRequest();
            }

            await _playlistSer.UpdatePlaylist(playlistDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaylist(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
