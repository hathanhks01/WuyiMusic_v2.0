using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuggestionController : ControllerBase
    {
        private readonly ISuggestionService _suggestionSer;
        public SuggestionController(ISuggestionService suggestionSer)
        {
            _suggestionSer = suggestionSer;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Suggestion>>> GetAllSuggestion()
        {
            var suggestions = await _suggestionSer.GetAllSuggestion();
            return Ok(suggestions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Suggestion>> GetSuggestionById(Guid id)
        {
            var suggestions = await _suggestionSer.GetByIdSuggestion(id);
            if (suggestions == null)
            {
                return NotFound();
            }
            return Ok(suggestions);
        }

        [HttpPost]
        public async Task<ActionResult<Suggestion>> CreateSuggestion(SuggestionDto suggestionDto)
        {
            await _suggestionSer.AddSuggestion(suggestionDto);
            return CreatedAtAction(nameof(GetSuggestionById), new { id = suggestionDto.SuggestionId }, suggestionDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSuggestion(Guid id, SuggestionDto suggestionDto)
        {
            if (id != suggestionDto.SuggestionId)
            {
                return BadRequest();
            }

            await _suggestionSer.UpdateSuggestion(suggestionDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSuggestion(Guid id)
        {
            throw new NotImplementedException();
        }
    }

}
