using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WuyiMusic_Services.Services.DropboxService;

namespace WuyiMusic_Services.IServices
{
    public interface IDropboxService
    {
        Task UploadFileAsync(Stream fileStream, string fileName);
        Task<DropboxFileInfo> UploadFileAsyncWithPath(Stream fileStream, string fileName);
        Task<string> GetSharedLinkAsync(string fileName);
        Task<string> RefreshTokenAsync();
        Task<string> GetPermanentSharedLinkAsync(string fileName);
    }
}
