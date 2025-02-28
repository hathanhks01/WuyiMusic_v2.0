using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using WuyiMusic_Services.IServices;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamController : ControllerBase
    {
        private readonly ITrackService _trackService;
        private readonly IHttpClientFactory _httpClientFactory;

        public StreamController(ITrackService trackService, IHttpClientFactory httpClientFactory)
        {
            _trackService = trackService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("play/{trackId}")]
        public async Task<IActionResult> StreamAudio(Guid trackId)
        {
            try
            {
                // Tăng lượt nghe khi phát nhạc
                await _trackService.IncrementListenCount(trackId);

                // Lấy thông tin track
                var track = await _trackService.GetByIdAsync(trackId);
                if (track == null)
                    return NotFound("Track not found");

                // Kiểm tra FilePath
                if (string.IsNullOrEmpty(track.FilePath))
                    return NotFound("Track file path not found");

                // Kiểm tra xem FilePath có phải URL trực tiếp không
                if (!Uri.TryCreate(track.FilePath, UriKind.Absolute, out Uri uriResult)
                    || (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
                {
                    return BadRequest("Invalid track file URL");
                }

                var httpClient = _httpClientFactory.CreateClient();

                // Chuyển tiếp header Range nếu có
                if (Request.Headers.ContainsKey("Range"))
                {
                    httpClient.DefaultRequestHeaders.Add("Range", Request.Headers["Range"].ToString());
                }

                // Lấy stream âm thanh từ URL
                var response = await httpClient.GetAsync(track.FilePath);

                if (!response.IsSuccessStatusCode)
                {
                    // Nếu URL hết hạn hoặc không hợp lệ, có thể cần refresh
                    // Thêm logic refresh URL tại đây nếu cần
                    return StatusCode((int)response.StatusCode, "Could not access audio file");
                }

                // Xử lý partial content
                if (Request.Headers.ContainsKey("Range") && response.StatusCode == System.Net.HttpStatusCode.PartialContent)
                {
                    Response.StatusCode = 206;
                    Response.Headers.Add("Accept-Ranges", "bytes");
                    if (response.Headers.Contains("Content-Range"))
                    {
                        Response.Headers.Add("Content-Range", response.Headers.GetValues("Content-Range").FirstOrDefault());
                    }
                }

                // Đặt headers
                Response.ContentType = "audio/mpeg";
                if (response.Content.Headers.ContentLength.HasValue)
                {
                    Response.ContentLength = response.Content.Headers.ContentLength.Value;
                }

                // Stream nội dung
                var stream = await response.Content.ReadAsStreamAsync();
                return new FileStreamResult(stream, "audio/mpeg");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error streaming audio: {ex.Message}");
            }
        }

        [HttpGet("metadata/{trackId}")]
        public async Task<IActionResult> GetTrackMetadata(Guid trackId)
        {
            try
            {
                var track = await _trackService.GetByIdAsync(trackId);
                if (track == null)
                    return NotFound("Track not found");

                return Ok(track);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting track metadata: {ex.Message}");
            }
        }
    }
}