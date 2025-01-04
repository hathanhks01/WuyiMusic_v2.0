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

        public DropboxService(HttpClient client, IConfiguration configuration)
        {
            httpClient = client;
            _configuration = configuration;
            accessToken = _configuration["DropBoxSettings:DropBoxToken"];
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
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception($"Lỗi từ Dropbox: {errorMessage}");
            }
        }

        public async Task<string> CreateSharedLinkAsync(string path)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropboxapi.com/2/sharing/create_shared_link_with_settings");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var body = new
            {
                path = path,
                settings = new
                {
                    requested_visibility = "public"
                }
            };

            request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception($"Lỗi từ Dropbox: {errorMessage}");
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
