using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LyricsController : ControllerBase
    {
        private readonly ILyricsService _lyricsSer;
        public LyricsController(ILyricsService lyricsSer)
        {
            _lyricsSer = lyricsSer;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lyrics>>> GetAllLyrics()
        {
            var lyrics = await _lyricsSer.GetAllLyrics();
            return Ok(lyrics);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Lyrics>> GetLyricsById(Guid id)
        {
            var lyrics = await _lyricsSer.GetByIdLyrics(id);
            if (lyrics == null)
            {
                return NotFound();
            }
            return Ok(lyrics);
        }

        [HttpPost]
        public async Task<ActionResult<Lyrics>> CreateLyrics(LyricsDto lyricsDto)
        {
            await _lyricsSer.AddLyrics(lyricsDto);
            return CreatedAtAction(nameof(GetLyricsById), new { id = lyricsDto.LyricsId }, lyricsDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLyrics(Guid id, LyricsDto lyricsDto)
        {
            if (id != lyricsDto.LyricsId)
            {
                return BadRequest();
            }

            await _lyricsSer.UpdateLyrics(lyricsDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLyrics(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
