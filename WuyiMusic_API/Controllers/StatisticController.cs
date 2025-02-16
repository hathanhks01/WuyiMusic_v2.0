using Microsoft.AspNetCore.Mvc;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        private readonly IUserStatisticsService _userStatisticsService;

        public StatisticController(IUserStatisticsService userStatisticsService)
        {
            _userStatisticsService = userStatisticsService;
        }

        [HttpGet("new-users-count")]
        public async Task<IActionResult> GetNewUsersCountByDate([FromQuery] DateTime date)
        {
            if (date == default)
            {
                return BadRequest("Invalid date provided.");
            }

            try
            {
                int count = await _userStatisticsService.GetNewUsersCountByDateAsync(date);
                return Ok(count);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, "Internal server error.");
            }
        }
        [HttpGet("new-users-count2")]
        public async Task<IActionResult> GetNewUsersCountByDateRange()
        {
            var counts = await _userStatisticsService.GetNewUsersCountForCurrentMonthAsync();
            return Ok(counts); 
        }
    }
}
