using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_Services.Services
{
    public class DropboxService:IDropboxService
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration _configuration;

        public DropboxService(HttpClient client, IConfiguration configuration)
        {
            httpClient = client;
            _configuration = configuration;
        }

        public async Task UploadFileAsync(Stream fileStream, string fileName)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://content.dropboxapi.com/2/files/upload");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["DropBoxSettings:DropBoxToken"]); // Kiểm tra token ở đây
            request.Headers.Add("Dropbox-API-Arg", JsonConvert.SerializeObject(new
            {
                path = $"/{fileName}",
                mode = "overwrite",
                autorename = true,
                mute = false,
            }));

            request.Content = new StreamContent(fileStream);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode(); // Kiểm tra phản hồi
        }
    }
}
