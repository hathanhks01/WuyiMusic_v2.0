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
        private readonly string _refreshToken;
        public DropboxService(HttpClient client, IConfiguration configuration, IHttpClientFactory hp)
        {
            httpClient = client;
            _httpClientFactory = hp;
            _configuration = configuration;
            accessToken = _configuration["DropBoxSettings:DropBoxToken"];
            _appSecret = _configuration["DropBoxSettings:Secret ID"];
            _appKey = _configuration["DropBoxSettings:Client ID"];
            _refreshToken = _configuration["DropBoxSettings:refreshToken"];
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
        public async Task<DropboxSharedLinkResult> GetPermanentSharedLinkAsync(string fileName)
        {
            try
            {
                // Kiểm tra xem liên kết đã tồn tại chưa
                var existingLink = await GetExistingSharedLinkAsync($"/WuyiMusic_Track/{fileName}");
                if (!string.IsNullOrEmpty(existingLink))
                {
                    var existingDirectLink = existingLink
                        .Replace("www.dropbox.com", "dl.dropboxusercontent.com")
                        .Replace("?dl=0", "")
                        .Replace("&dl=0", "");
                    return new DropboxSharedLinkResult
                    {
                        SharedLink = existingLink,
                        DirectLink = existingDirectLink
                    };
                }

                // Nếu không có liên kết, tạo liên kết mới
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropboxapi.com/2/sharing/create_shared_link_with_settings");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var dropboxApiArg = new
                {
                    path = $"/WuyiMusic_Track/{fileName}",
                    settings = new
                    {
                        requested_visibility = "public",
                        audience = "public",
                        access = "viewer",
                        allow_download = true
                    }
                };

                request.Content = new StringContent(JsonConvert.SerializeObject(dropboxApiArg), Encoding.UTF8, "application/json");

                var response = await httpClient.SendAsync(request);

                // Kiểm tra token hết hạn, nếu cần refresh token
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var newTokenResponse = await RefreshTokenAsync();
                    var newTokenData = JsonConvert.DeserializeObject<Dictionary<string, string>>(newTokenResponse);
                    accessToken = newTokenData["access_token"];

                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    response = await httpClient.SendAsync(request);
                }

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Lỗi từ Dropbox: {errorMessage}");
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
                string sharedLink = jsonResponse.url;
                string directLink = sharedLink
                    .Replace("www.dropbox.com", "dl.dropboxusercontent.com")
                    .Replace("?dl=0", "")
                    .Replace("&dl=0", "");

                return new DropboxSharedLinkResult
                {
                    SharedLink = sharedLink,
                    DirectLink = directLink
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo shared link: {ex.Message}");
            }
        }


        public async Task<DropboxSharedLinkResult> GetPermanentSharedLinkImageAsync(string fileName)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropboxapi.com/2/sharing/create_shared_link_with_settings");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var dropboxApiArg = new
                {
                    path = $"/WuyiMusic_Images/{fileName}",
                    settings = new
                    {
                        requested_visibility = "public" // Change to a valid visibility setting
                    }
                };

                request.Content = new StringContent(
                    JsonConvert.SerializeObject(dropboxApiArg),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
                    string sharedLink = jsonResponse.url;
                    string directLink = sharedLink.Replace("www.dropbox.com", "dl.dropboxusercontent.com");

                    return new DropboxSharedLinkResult
                    {
                        SharedLink = sharedLink,
                        DirectLink = directLink
                    };
                }

                throw new Exception($"Error: {await response.Content.ReadAsStringAsync()}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating shared link: {ex.Message}");
            }
        }



        // Update GetExistingSharedLinkAsync to handle full paths
        private async Task<string> GetExistingSharedLinkAsync(string fullPath)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropboxapi.com/2/sharing/list_shared_links");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var dropboxApiArg = new
            {
                path = fullPath,
                direct_only = true
            };

            request.Content = new StringContent(JsonConvert.SerializeObject(dropboxApiArg), Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
                if (jsonResponse.links.Count > 0)
                {
                    var link = jsonResponse.links[0].url;
                    if (link is string urlString)
                    {
                        return urlString;
                    }
                }
            }

            return null;
        }



        /// image
        /// 

        public async Task<DropboxFileInfo> UploadImageAsync(Stream imageStream, string imageName)
        {
            // Đảm bảo tên file có phần mở rộng
            if (!Path.HasExtension(imageName))
            {
                throw new ArgumentException("Tên file phải bao gồm phần mở rộng (ví dụ: .jpg, .png, .gif, .webp)");
            }

            // Xác định loại MIME dựa trên phần mở rộng file
            string mimeType = GetImageMimeType(Path.GetExtension(imageName).ToLowerInvariant());

            // Tạo đường dẫn đầy đủ cho thư mục ảnh
            var fullPath = $"/WuyiMusic_Images/{imageName}";

            // Kiểm tra và reset vị trí stream
            if (imageStream.CanSeek)
            {
                imageStream.Position = 0;
            }

            if (imageStream.Length == 0)
            {
                throw new Exception("Luồng ảnh trống");
            }

            using (var memoryStream = new MemoryStream())
            {
                // Sao chép stream ảnh vào memory stream
                await imageStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var request = CreateImageUploadRequest(memoryStream, imageName, mimeType);
                var response = await httpClient.SendAsync(request);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var newTokenResponse = await RefreshTokenAsync();
                    var newTokenData = JsonConvert.DeserializeObject<Dictionary<string, string>>(newTokenResponse);
                    accessToken = newTokenData["access_token"];
                    memoryStream.Position = 0;
                    request = CreateImageUploadRequest(memoryStream, imageName, mimeType);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    response = await httpClient.SendAsync(request);
                }
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Upload ảnh lên Dropbox thất bại: {errorMessage}");
                }

                // Đọc thông tin phản hồi từ Dropbox
                var responseBody = await response.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
                string filePath = jsonResponse.path_display;

                return new DropboxFileInfo
                {
                    Path = filePath,
                    Name = imageName
                };
            }
        }

        private string GetImageMimeType(string fileExtension)
        {
            // Mapping các phần mở rộng ảnh phổ biến với MIME type
            return fileExtension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".bmp" => "image/bmp",
                ".tiff" or ".tif" => "image/tiff",
                ".svg" => "image/svg+xml",
                ".heic" or ".heif" => "image/heif",
                _ => throw new ArgumentException($"Định dạng ảnh không được hỗ trợ: {fileExtension}")
            };
        }

        private HttpRequestMessage CreateImageUploadRequest(Stream imageStream, string imageName, string mimeType)
        {
            if (!imageStream.CanRead)
            {
                throw new Exception("Không thể đọc luồng dữ liệu.");
            }

            var request = new HttpRequestMessage(HttpMethod.Post, "https://content.dropboxapi.com/2/files/upload");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var dropboxApiArg = new
            {
                path = $"/WuyiMusic_Images/{imageName}",
                mode = "overwrite",
                autorename = true,
                mute = false
            };

            request.Headers.Add("Dropbox-API-Arg", JsonConvert.SerializeObject(dropboxApiArg));

            var streamContent = new StreamContent(imageStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            request.Content = streamContent;

            return request;
        }



        //replace file 
        /// <summary>
        /// Thay thế file trên Dropbox. Nếu đường dẫn mới khác với đường dẫn cũ thì sẽ xóa file cũ (nếu có) rồi upload file mới.
        /// Bạn có thể truyền folder là "/WuyiMusic_Track" hoặc "/WuyiMusic_Images" tùy theo mục đích sử dụng.
        /// </summary>
        /// <param name="fileStream">Luồng file mới</param>
        /// <param name="oldFilePath">
        /// Đường dẫn file cũ trên Dropbox (ví dụ: "/WuyiMusic_Track/oldFile.mp3"). 
        /// Nếu không cần xóa file cũ thì truyền null hoặc chuỗi rỗng.
        /// </param>
        /// <param name="newFileName">Tên file mới (ví dụ: "newFile.mp3")</param>
        /// <param name="folder">Folder trên Dropbox (ví dụ: "/WuyiMusic_Track" hoặc "/WuyiMusic_Images")</param>
        /// <returns>Thông tin file sau khi upload</returns>
        public async Task<DropboxFileInfo> ReplaceFileAsync(Stream fileStream, string oldFilePath, string newFileName, string folder)
        {
            // Kiểm tra và đặt lại vị trí stream
            if (fileStream == null || fileStream.Length == 0)
            {
                throw new Exception("File stream is empty");
            }
            if (fileStream.CanSeek)
            {
                fileStream.Position = 0;
            }

            // Nếu có file cũ và đường dẫn của file mới khác file cũ,
            // thì xóa file cũ (nếu bạn cần loại bỏ file cũ khỏi Dropbox)
            string newFilePath = $"{folder}/{newFileName}";
            if (!string.IsNullOrWhiteSpace(oldFilePath) &&
                !oldFilePath.Equals(newFilePath, StringComparison.OrdinalIgnoreCase))
            {
                await DeleteFileAsync(oldFilePath);
            }

            // Dùng MemoryStream để có thể reset stream nếu cần retry
            using (var memoryStream = new MemoryStream())
            {
                await fileStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                // Tạo yêu cầu upload với mode overwrite (dù file chưa tồn tại, mode này cũng an toàn)
                var request = CreateReplaceUploadRequest(memoryStream, newFileName, folder);
                var response = await httpClient.SendAsync(request);

                // Nếu token hết hạn, refresh token và thử lại
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var newTokenResponse = await RefreshTokenAsync();
                    var newTokenData = JsonConvert.DeserializeObject<Dictionary<string, string>>(newTokenResponse);
                    accessToken = newTokenData["access_token"];

                    // Reset lại stream và tạo lại request
                    memoryStream.Position = 0;
                    request = CreateReplaceUploadRequest(memoryStream, newFileName, folder);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    response = await httpClient.SendAsync(request);
                }

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Dropbox file replacement failed: {errorMessage}");
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
                string filePath = jsonResponse.path_display;

                return new DropboxFileInfo
                {
                    Path = filePath,
                    Name = newFileName
                };
            }
        }

        /// <summary>
        /// Tạo yêu cầu upload file với mode overwrite tại folder chỉ định
        /// </summary>
        private HttpRequestMessage CreateReplaceUploadRequest(Stream fileStream, string fileName, string folder)
        {
            if (!fileStream.CanRead)
            {
                throw new Exception("Cannot read from the provided stream.");
            }

            var request = new HttpRequestMessage(HttpMethod.Post, "https://content.dropboxapi.com/2/files/upload");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var dropboxApiArg = new
            {
                path = $"{folder}/{fileName}",
                mode = "overwrite",       // Ghi đè file nếu tồn tại
                autorename = false,       // Không tự động đổi tên
                mute = false
            };

            request.Headers.Add("Dropbox-API-Arg", JsonConvert.SerializeObject(dropboxApiArg));

            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            request.Content = streamContent;

            return request;
        }

        /// <summary>
        /// Xóa file trên Dropbox tại đường dẫn được chỉ định
        /// </summary>
        public async Task DeleteFileAsync(string filePath)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropboxapi.com/2/files/delete_v2");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var contentObj = new { path = filePath };
            request.Content = new StringContent(JsonConvert.SerializeObject(contentObj), Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to delete file: {errorMessage}");
            }
        }

        public async Task<string> GetInternalPathFromSharedLinkAsync(string sharedLink)
        {
            // Create the initial request message
            HttpRequestMessage request = CreateGetSharedLinkMetadataRequest(sharedLink, accessToken);
            var response = await httpClient.SendAsync(request);

            // If token expired, refresh and retry with a new request message
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var newTokenResponse = await RefreshTokenAsync();
                var newTokenData = JsonConvert.DeserializeObject<Dictionary<string, string>>(newTokenResponse);
                accessToken = newTokenData["access_token"];

                // Create a new request with the updated token
                request = CreateGetSharedLinkMetadataRequest(sharedLink, accessToken);
                response = await httpClient.SendAsync(request);
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to get metadata from shared link: {errorMessage}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
            string internalPath = jsonResponse.path_lower;
            return internalPath;
        }

        private HttpRequestMessage CreateGetSharedLinkMetadataRequest(string sharedLink, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropboxapi.com/2/sharing/get_shared_link_metadata");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var requestBody = new { url = sharedLink };
            request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            return request;
        }



        /// <summary>
        /// Deletes a file from Dropbox given its full path
        /// </summary>
        /// <param name="filePath">Full path to the file in Dropbox (e.g., "/WuyiMusic_Track/song.mp3" or "/WuyiMusic_Images/image.jpg")</param>
        /// <param name="retryOnTokenExpired">Whether to retry the operation if the token has expired. Default is true.</param>
        /// <returns>True if deletion was successful, false if the file doesn't exist</returns>
        /// <exception cref="ArgumentException">Thrown when filePath is null or empty</exception>
        /// <exception cref="Exception">Thrown when deletion fails for reasons other than file not found</exception>
        public async Task<bool> DeleteFileFromDropboxAsync(string filePath, bool retryOnTokenExpired = true)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropboxapi.com/2/files/delete_v2");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var contentObj = new { path = filePath };
                request.Content = new StringContent(
                    JsonConvert.SerializeObject(contentObj),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await httpClient.SendAsync(request);

                // Handle token expiration
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && retryOnTokenExpired)
                {
                    var newTokenResponse = await RefreshTokenAsync();
                    var newTokenData = JsonConvert.DeserializeObject<Dictionary<string, string>>(newTokenResponse);
                    accessToken = newTokenData["access_token"];

                    // Retry with new token
                    request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropboxapi.com/2/files/delete_v2");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    request.Content = new StringContent(
                        JsonConvert.SerializeObject(contentObj),
                        Encoding.UTF8,
                        "application/json"
                    );
                    response = await httpClient.SendAsync(request);
                }

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                dynamic errorJson = JsonConvert.DeserializeObject(errorContent);

                // Check if the error is "path_lookup/not_found" which means the file doesn't exist
                if (errorJson?.error?.path?[".tag"]?.ToString() == "not_found")
                {
                    return false;
                }

                throw new Exception($"Failed to delete file from Dropbox: {errorContent}");
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is JsonException)
            {
                throw new Exception($"Error while deleting file from Dropbox: {ex.Message}", ex);
            }
        }

    }
    public class DropboxSharedLinkResult
    {
        public string SharedLink { get; set; }
        public string DirectLink { get; set; }
    }

}
