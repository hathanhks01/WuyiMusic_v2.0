using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DropboxController : ControllerBase
    {
        private readonly IDropboxService _dropboxService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DropboxController> _logger;
        private readonly string _appKey = "lixa5ath8kd882n"; // Should be moved to configuration
        private readonly string _appSecret = "uw7d29k9fmfvle0"; // Should be moved to configuration

        public DropboxController(
            IDropboxService dropboxService,
            IHttpClientFactory httpClientFactory,
            ILogger<DropboxController> logger)
        {
            _dropboxService = dropboxService;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // Upload music file
        [HttpPost("upload")]
        public async Task<IActionResult> UploadMusic(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Tệp không hợp lệ.");
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0; // Reset the stream position to the beginning

                    // Kiểm tra kích thước của stream
                    if (stream.Length == 0)
                    {
                        return BadRequest("Tệp rỗng không thể upload.");
                    }

                    await _dropboxService.UploadFileAsync(stream, file.FileName);
                }
                return Ok("Upload thành công.");
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError($"HTTP error uploading file: {httpEx.Message}");
                return StatusCode(500, "Đã xảy ra lỗi HTTP khi upload tệp: " + httpEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading file: {ex.Message}");
                return StatusCode(500, "Đã xảy ra lỗi khi upload tệp: " + ex.Message);
            }
        }


        [HttpPost("get-token")]
        public async Task<IActionResult> GetToken([FromBody] DropboxAuthRequest authRequest)
        {
            // Kiểm tra yêu cầu đầu vào
            if (string.IsNullOrEmpty(authRequest.AccessCode))
            {
                return BadRequest("Access code is required.");
            }

            using (var httpClient = _httpClientFactory.CreateClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropbox.com/oauth2/token"))
                {
                    var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_appKey}:{_appSecret}"));
                    request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

                    var contentList = new List<string>
                    {
                        $"code={authRequest.AccessCode}",
                        "grant_type=authorization_code"
                    };

                    request.Content = new StringContent(string.Join("&", contentList));
                    request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

                    // Gửi yêu cầu và nhận phản hồi
                    var response = await httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        // Đọc và trả lại dữ liệu JSON
                        var result = await response.Content.ReadAsStringAsync();
                        return Ok(result);
                    }

                    // Nếu có lỗi từ API Dropbox
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, errorResponse);
                }

            }
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            try
            {
                var response = await _dropboxService.RefreshTokenAsync();
                return Ok(response); // Trả về response từ Dropbox API
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Trả về lỗi nếu có vấn đề
            }
        }



        // DTO for the token request
        public class DropboxAuthRequest
        {
            public string AccessCode { get; set; }
        }
    }
}
