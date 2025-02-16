using Dropbox.Api.Files;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models;
using WuyiMusic_DAL.Reponsitories;
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
                throw new ArgumentException("Tệp không hợp lệ.");
            }

            try
            {
                // Thêm track vào database với duration đã được format
                await _trackRepository.AddAsync(track, file);

                // Upload file lên Dropbox và cập nhật FilePath
                using (var stream = file.OpenReadStream())
                {
                    await _dropboxService.UploadFileAsync(stream, file.FileName);
                    var sharedLink = await _dropboxService.GetPermanentSharedLinkAsync(file.FileName);
                    track.FilePath = sharedLink.DirectLink; // link direct
                    track.MetaLink = sharedLink.SharedLink;
                }

                // Cập nhật track với FilePath mới
                await _trackRepository.UpdateAsync(track);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể thêm track. " + ex.Message);
            }
        }

        public async Task UpdateAsync(Track updatedTrack, IFormFile file = null)
        {
            // Lấy thông tin track hiện tại từ database
            var existingTrack = await _trackRepository.GetByIdAsync(updatedTrack.TrackId);
            if (existingTrack == null)
            {
                throw new ArgumentException("Không tìm thấy track.");
            }

            // Cập nhật các thuộc tính cơ bản
            existingTrack.Title = updatedTrack.Title;
            existingTrack.Duration = updatedTrack.Duration;
            existingTrack.AlbumId = updatedTrack.AlbumId;
            existingTrack.ArtistId = updatedTrack.ArtistId;
            existingTrack.TrackImage = updatedTrack.TrackImage;

            // Xử lý nếu có file mới
            if (file != null && file.Length > 0)
            {
                try
                {
                    // Lấy đường dẫn nội bộ từ link chia sẻ cũ
                    string oldInternalPath = await _dropboxService.GetInternalPathFromSharedLinkAsync(existingTrack.MetaLink);

                    using (var stream = file.OpenReadStream())
                    {
                        // Thay thế file trên Dropbox
                        var folder = "/WuyiMusic_Track";
                        var newFileName = file.FileName;

                        var dropboxFileInfo = await _dropboxService.ReplaceFileAsync(
                            stream,
                            oldInternalPath,
                            newFileName,
                            folder
                        );

                        // Lấy link mới
                        var sharedLinkResult = await _dropboxService.GetPermanentSharedLinkAsync(newFileName);

                        // Cập nhật metadata
                        existingTrack.FilePath = sharedLinkResult.DirectLink;
                        existingTrack.MetaLink = sharedLinkResult.SharedLink;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi cập nhật file: {ex.Message}");
                }
            }

            // Lưu thay đổi vào database
            await _trackRepository.UpdateAsync(existingTrack);
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var trackToDelete = await _trackRepository.GetByIdAsync(id);

                if (trackToDelete != null)
                {
                    if (!string.IsNullOrEmpty(trackToDelete.MetaLink))
                    {
                        await _dropboxService.DeleteFileFromDropboxAsync(trackToDelete.MetaLink);
                    }
                    else
                    {
                        // Log that MetaLink is null or empty
                        Console.WriteLine($"MetaLink is null or empty for track ID: {id}");
                    }

                    await _trackRepository.DeleteAsync(id);
                }
                else
                {
                    Console.WriteLine($"Track with ID: {id} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting track with ID: {id}. Error: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Track>> GetFavoriteTracksAsync(Guid userId)
        {
            return await _trackRepository.GetFavoriteTracksAsync(userId);
        }
        public async Task<List<Track>> GetTrackRankingByListenCount(DateTime startDate, DateTime endDate, int topCount = 10)
        {
            return await _trackRepository.GetTrackRankingByListenCount(startDate, endDate, topCount);
        }

     

        public async Task<SearchResultDto> SearchAsync(string searchTerm)
        {
            return await _trackRepository.SearchAsync(searchTerm);
        }

        public Task<IEnumerable<Track>> GetRandom6Track(Guid artistId)
        {
            return _trackRepository.GetRandom6Track(artistId);
        }

        public async Task IncrementListenCount(Guid trackId)
        {
           await _trackRepository.IncrementListenCount(trackId);
        }
    }
}
