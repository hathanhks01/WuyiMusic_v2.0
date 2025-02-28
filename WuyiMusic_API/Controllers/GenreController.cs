using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WuyiMusic_DAL.Models;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        // GET: api/genre
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genre>>> GetGenres()
        {
            var genres = await _genreService.GetAllGenres();
            return Ok(genres);
        }

    

        // POST api/genre
        [HttpPost]
        public async Task<ActionResult<Genre>> Post([FromBody] Genre genre)
        {
            if (genre == null)
            {
                return BadRequest("Genre cannot be null");
            }

            var createdGenre = await _genreService.AddGenre(genre);
            return Ok(createdGenre);
        }

        // DELETE api/genre/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _genreService.DeleteGenre(id);
            return NoContent();
        }
    }
}
