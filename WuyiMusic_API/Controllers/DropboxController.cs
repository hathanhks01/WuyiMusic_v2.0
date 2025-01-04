using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WuyiMusic_Services.IServices;
using static Dropbox.Api.TeamLog.EventCategory;

namespace WuyiMusic_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DropboxController : ControllerBase
    {
        private readonly IDropboxService _dropboxService;
        public DropboxController(IDropboxService dropboxService)
        {
            _dropboxService = dropboxService;
        }

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
                    stream.Position = 0; // Đặt lại vị trí luồng về đầu
                    await _dropboxService.UploadFileAsync(stream, file.FileName);
                }
                return Ok("Upload thành công.");
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để kiểm tra
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Đã xảy ra lỗi khi upload tệp.");
            }
        }
    }
    }
