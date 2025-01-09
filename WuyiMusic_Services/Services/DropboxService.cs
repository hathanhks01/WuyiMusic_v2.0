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

            if (fileStream.Length == 0)
            {
                throw new Exception("Tệp rỗng không thể upload.");
            }

            request.Content = new StreamContent(fileStream);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var response = await httpClient.SendAsync(request);

            // Nếu access token hết hạn (401), thử làm mới token và thử lại
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();

                // Kiểm tra nếu là lỗi do access token hết hạn (401)
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Làm mới token
                    var newAccessTokenResponse = await RefreshTokenAsync();
                    var newAccessTokenData = JsonConvert.DeserializeObject<Dictionary<string, string>>(newAccessTokenResponse);
                    accessToken = newAccessTokenData["access_token"]; // Cập nhật access token mới

                    // Gửi lại yêu cầu với access token mới
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    response = await httpClient.SendAsync(request);

                    // Nếu lần này không thành công thì ném lỗi
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

        private HttpRequestMessage CreateUploadRequest(Stream fileStream, string fileName)
        {
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

            if (fileStream.Length == 0)
            {
                throw new Exception("Tệp rỗng không thể upload.");
            }

            request.Content = new StreamContent(fileStream);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return request;
        }


    }
}
