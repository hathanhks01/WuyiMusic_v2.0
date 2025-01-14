using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_Services.Services
{
    public class DropboxService : IDropboxService
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration _configuration;
        private string accessToken;
        private string _appSecret;
        private string _appKey;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _refreshToken = "HL4kbLkwefIAAAAAAAAAAV3M7Fwkm--BTncQsPX50RWUj_cwERpLvjW-YYH7aP66";
        public DropboxService(HttpClient client, IConfiguration configuration, IHttpClientFactory hp)
        {
            httpClient = client;
            _httpClientFactory=hp;
           _configuration = configuration;
            accessToken = _configuration["DropBoxSettings:DropBoxToken"];
            _appSecret = _configuration["DropBoxSettings:Secret ID"];
            _appKey= _configuration["DropBoxSettings:Client ID"];
        }

        public async Task UploadFileAsync(Stream fileStream, string fileName)
        {
            if (fileStream.CanSeek)
            {
                fileStream.Position = 0;
            }
            if (fileStream.Length == 0)
            {
                throw new Exception("File stream is empty");
            }
            using (var memoryStream = new MemoryStream())
            {
                await fileStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;  
                var request = CreateUploadRequest(memoryStream, fileName);
                var response = await httpClient.SendAsync(request);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var newTokenResponse = await RefreshTokenAsync();
                    var newTokenData = JsonConvert.DeserializeObject<Dictionary<string, string>>(newTokenResponse);
                    accessToken = newTokenData["access_token"];
                    // Reset stream position for retry
                    memoryStream.Position = 0;
                    // Retry with new token
                    request = CreateUploadRequest(memoryStream, fileName);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    response = await httpClient.SendAsync(request);
                }

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Dropbox upload failed: {errorMessage}");
                }
            }
        }

        private HttpRequestMessage CreateUploadRequest(Stream fileStream, string fileName)
        {
            if (!fileStream.CanRead)
            {
                throw new Exception("Cannot read from the provided stream.");
            }

            var request = new HttpRequestMessage(HttpMethod.Post, "https://content.dropboxapi.com/2/files/upload");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var dropboxApiArg = new
            {
                path = $"/WuyiMusic_Track/{fileName}",
                mode = "overwrite",
                autorename = true,
                mute = false
            };

            request.Headers.Add("Dropbox-API-Arg", JsonConvert.SerializeObject(dropboxApiArg));

            // Create new StreamContent from the stream
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            request.Content = streamContent;

            return request;
        }

        public async Task<string> RefreshTokenAsync()
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.dropbox.com/oauth2/token"))
                {
                    var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_appKey}:{_appSecret}"));
                    request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

                    // Đảm bảo format đúng, sử dụng StringContent thay vì List<string>
                    var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "refresh_token", _refreshToken },
                { "grant_type", "refresh_token" }
            });

                    request.Content = content;
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                    var response = await httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        return responseBody; // Trả về kết quả từ API
                    }

                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to refresh token: {errorMessage}");
                }
            }
        }

        public class DropboxFileInfo
        {
            public string Path { get; set; }
            public string Name { get; set; }
        }

        public async Task<DropboxFileInfo> UploadFileAsyncWithPath(Stream fileStream, string fileName)
        {
            var request = CreateUploadRequest(fileStream, fileName);
            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var newAccessTokenResponse = await RefreshTokenAsync();
                    var newAccessTokenData = JsonConvert.DeserializeObject<Dictionary<string, string>>(newAccessTokenResponse);
                    accessToken = newAccessTokenData["access_token"];

                    // Tạo yêu cầu mới với access token mới
                    request = CreateUploadRequest(fileStream, fileName);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Lỗi từ Dropbox: {await response.Content.ReadAsStringAsync()}");
                    }
                }
                else
                {
                    throw new Exception($"Lỗi từ Dropbox: {errorMessage}");
                }
            }

            // Đọc kết quả trả về từ Dropbox
            var responseBody = await response.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
            string filePath = jsonResponse.path_display;

            return new DropboxFileInfo
            {
                Path = filePath,
                Name = fileName
            };
        }

        
        public async Task<string> GetSharedLinkAsync(string fileName)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropboxapi.com/2/files/get_temporary_link");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var dropboxApiArg = new
            {
                path = $"/WuyiMusic_Track/{fileName}"
            };

            request.Content = new StringContent(JsonConvert.SerializeObject(dropboxApiArg), Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception($"Lỗi từ Dropbox: {errorMessage}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
            return jsonResponse.link; 
        }
        public async Task<string> GetPermanentSharedLinkAsync(string fileName)
        {
            // Kiểm tra xem liên kết đã tồn tại chưa
            var existingLink = await GetExistingSharedLinkAsync(fileName);
            if (!string.IsNullOrEmpty(existingLink))
            {
                return existingLink; // Trả về liên kết đã tồn tại
            }

            // Nếu không có liên kết, tạo liên kết mới
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropboxapi.com/2/sharing/create_shared_link_with_settings");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var dropboxApiArg = new
            {
                path = $"/WuyiMusic_Track/{fileName}",
                settings = new
                {
                    requested_visibility = "public" // Thiết lập quyền truy cập công khai
                }
            };

            request.Content = new StringContent(JsonConvert.SerializeObject(dropboxApiArg), Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception($"Lỗi từ Dropbox: {errorMessage}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
            string sharedLink = jsonResponse.url;

            // Chuyển đổi liên kết chia sẻ thành liên kết có thể phát
            string audioLink = sharedLink.Replace("?dl=0", "?raw=1");

            return audioLink; // Trả về liên kết có thể phát
        }


        private async Task<string> GetExistingSharedLinkAsync(string fileName)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropboxapi.com/2/sharing/list_shared_links");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var dropboxApiArg = new
            {
                path = $"/WuyiMusic_Track/{fileName}",
                direct_only = true // Chỉ lấy liên kết trực tiếp
            };

            request.Content = new StringContent(JsonConvert.SerializeObject(dropboxApiArg), Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
                if (jsonResponse.links.Count > 0)
                {
                    // Kiểm tra xem url có phải là chuỗi không
                    var link = jsonResponse.links[0].url;
                    if (link is string urlString)
                    {
                        return urlString.Replace("?dl=0", "?raw=1");
                    }
                }
            }

            return null; // Không có liên kết nào tồn tại
        }



    }
}
