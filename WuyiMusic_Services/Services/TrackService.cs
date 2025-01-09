using Dropbox.Api.Files;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_Services.Services
{
    public class TrackService : ITrackService
    {
        private readonly ITrackRepository _trackRepository;
        private readonly IDropboxService _dropboxService;

        public TrackService(ITrackRepository trackRepository, IDropboxService dropboxService)
        {
            _trackRepository = trackRepository;
            _dropboxService = dropboxService; // Inject DropboxService
        }

        public async Task<IEnumerable<Track>> GetAllAsync()
        {
            return await _trackRepository.GetAllAsync();
        }

        public async Task<Track> GetByIdAsync(Guid id)
        {
            return await _trackRepository.GetByIdAsync(id);
        }

        public async Task AddTrackAsync(Track track, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new Exception("Tệp không hợp lệ.");
            }

            using (var stream = file.OpenReadStream())
            {
                // Tải tệp lên Dropbox và lấy thông tin tệp
                var dropboxFileInfo = await _dropboxService.UploadFileAsyncWithPath(stream, file.FileName);
                track.FilePath = dropboxFileInfo.Path; // Lưu đường dẫn vào Track
            }

            // Thêm track vào cơ sở dữ liệu
            await _trackRepository.AddAsync(track);
        }

        public async Task UpdateAsync(Track track)
        {
            await _trackRepository.UpdateAsync(track);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _trackRepository.DeleteAsync(id);
        }
    }
}
