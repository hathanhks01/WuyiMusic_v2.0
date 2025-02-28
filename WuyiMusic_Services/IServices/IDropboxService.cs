using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_Services.Services;
using static WuyiMusic_Services.Services.DropboxService;

namespace WuyiMusic_Services.IServices
{
    public interface IDropboxService
    {
        Task UploadFileAsync(Stream fileStream, string fileName);
        Task<DropboxFileInfo> UploadFileAsyncWithPath(Stream fileStream, string fileName);
        Task<DropboxFileInfo> ReplaceFileAsync(Stream fileStream, string oldFilePath, string newFileName, string folder);
        Task<string> RefreshTokenAsync();
        Task<DropboxSharedLinkResult> GetPermanentSharedLinkAsync(string fileName);
        Task<DropboxSharedLinkResult> GetPermanentSharedLinkImageAsync(string fileName);
        Task<DropboxFileInfo> UploadImageAsync(Stream imageStream, string imageName);

        Task<string> GetInternalPathFromSharedLinkAsync(string sharedLink);
        Task DeleteFileAsync(string filePath);
        Task<bool> DeleteFileFromDropboxAsync(string filePath, bool retryOnTokenExpired = true);
    }
}
