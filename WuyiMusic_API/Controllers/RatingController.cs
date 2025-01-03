using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingSer;
        public RatingController(IRatingService ratingSer)
        {
            _ratingSer = ratingSer;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rating>>> GetAllRating()
        {
            var ratings = await _ratingSer.GetAllRating();
            return Ok(ratings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Artist>> GetRatingById(Guid id)
        {
            var ratings = await _ratingSer.GetByIdRating(id);
            if (ratings == null)
            {
                return NotFound();
            }
            return Ok(ratings);
        }

        [HttpPost]
        public async Task<ActionResult<Rating>> CreateRating(RatingDto ratingDto)
        {
            await _ratingSer.AddRating(ratingDto);
            return CreatedAtAction(nameof(GetRatingById), new { id = ratingDto.RatingId }, ratingDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRating(Guid id, RatingDto ratingDto)
        {
            if (id != ratingDto.RatingId)
            {
                return BadRequest();
            }

            await _ratingSer.UpdateRating(ratingDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRating(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
