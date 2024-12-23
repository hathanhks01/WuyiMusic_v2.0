using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentSer;
        public CommentController(ICommentService commentSer)
        {
            _commentSer = commentSer;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetAllComment()
        {
            var comments = await _commentSer.GetAllComment();
            return Ok(comments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetCommentById(Guid id)
        {
            var comments = await _commentSer.GetByIdComment(id);
            if (comments == null)
            {
                return NotFound();
            }
            return Ok(comments);
        }

        [HttpPost]
        public async Task<ActionResult<Comment>> CreateComment(CommentDto commentDto)
        {
            await _commentSer.AddComment(commentDto);
            return CreatedAtAction(nameof(GetCommentById), new { id = commentDto.CommentId }, commentDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(Guid id, CommentDto commentDto)
        {
            if (id != commentDto.CommentId)
            {
                return BadRequest();
            }

            await _commentSer.UpdateComment(commentDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
