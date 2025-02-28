using Dropbox.Api.Files;
using Microsoft.AspNetCore.Http;
using NAudio.Wave;
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

        public async Task AddTrackAsync(Track track, IFormFile file, IFormFile imageFile)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Tệp không hợp lệ.");
            }

            try
            {
                await _trackRepository.AddAsync(track, file);
                using (var stream = file.OpenReadStream())
                {
                    await _dropboxService.UploadFileAsync(stream, file.FileName);
                    var sharedLink = await _dropboxService.GetPermanentSharedLinkAsync(file.FileName);
                    track.FilePath = sharedLink.DirectLink; // link direct
                    track.MetaLink = sharedLink.SharedLink;
                }
                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var imageStream = imageFile.OpenReadStream())
                    {
                        var imageInfo = await _dropboxService.UploadImageAsync(imageStream, imageFile.FileName);
                        var imageSharedLink = await _dropboxService.GetPermanentSharedLinkImageAsync(imageFile.FileName);
                        track.TrackImage = imageSharedLink.DirectLink;
                        track.MetaLinkImage = imageSharedLink.SharedLink;
                    }
                }
                await _trackRepository.UpdateAsync(track);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể thêm track. " + ex.Message);
            }
        }

        public async Task UpdateAsync(Track updatedTrack, IFormFile file = null, IFormFile imageFile = null)
        {
            var existingTrack = await _trackRepository.GetByIdAsync(updatedTrack.TrackId);
            if (existingTrack == null)
            {
                throw new ArgumentException("Không tìm thấy track.");
            }

            // Cập nhật các thuộc tính cơ bản (không bao gồm TrackImage)
            existingTrack.Title = updatedTrack.Title;
            existingTrack.Duration = updatedTrack.Duration;
            existingTrack.AlbumId = updatedTrack.AlbumId;
            existingTrack.ArtistId = updatedTrack.ArtistId;

            // Xử lý file audio mới
            if (file != null && file.Length > 0)
            {
                try
                {
                    string oldInternalPath = await _dropboxService.GetInternalPathFromSharedLinkAsync(existingTrack.MetaLink);
                    using (var stream = file.OpenReadStream())
                    {
                        var folder = "/WuyiMusic_Track";
                        var newFileName = file.FileName;

                        var dropboxFileInfo = await _dropboxService.ReplaceFileAsync(
                            stream,
                            oldInternalPath,
                            newFileName,
                            folder
                        );
                        var tempFilePath = Path.GetTempFileName();
                        try
                        {
                            using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            using (var reader = new AudioFileReader(tempFilePath))
                            {
                                var duration = reader.TotalTime;
                                if (duration.Hours > 0)
                                {
                                    existingTrack.Duration = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                        duration.Hours,
                                        duration.Minutes,
                                        duration.Seconds);
                                }
                                else
                                {
                                    existingTrack.Duration = string.Format("{0:D2}:{1:D2}",
                                        duration.Minutes,
                                        duration.Seconds);
                                }
                            }
                        }
                        finally
                        {
                            if (File.Exists(tempFilePath))
                            {
                                File.Delete(tempFilePath);
                            }
                        }

                        var sharedLinkResult = await _dropboxService.GetPermanentSharedLinkAsync(newFileName);
                        existingTrack.FilePath = sharedLinkResult.DirectLink;
                        existingTrack.MetaLink = sharedLinkResult.SharedLink;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi cập nhật file audio: {ex.Message}");
                }
            }

            // Xử lý file ảnh mới
            if (imageFile != null && imageFile.Length > 0)
            {
                try
                {
                    // Lấy đường dẫn ảnh cũ từ Dropbox
                    string oldImageInternalPath = await _dropboxService.GetInternalPathFromSharedLinkAsync(existingTrack.MetaLinkImage);

                    using (var imageStream = imageFile.OpenReadStream())
                    {
                        var imageFolder = "/WuyiMusic_Images";
                        var newImageFileName = imageFile.FileName;

                        // Upload ảnh mới và xóa ảnh cũ (nếu cần)
                        var imageDropboxInfo = await _dropboxService.ReplaceFileAsync(
                            imageStream,
                            oldImageInternalPath,
                            newImageFileName,
                            imageFolder
                        );

                        // Lấy liên kết mới cho ảnh
                        var imageSharedLinkResult = await _dropboxService.GetPermanentSharedLinkImageAsync(newImageFileName);
                        existingTrack.TrackImage = imageSharedLinkResult.DirectLink;
                        existingTrack.MetaLinkImage = imageSharedLinkResult.SharedLink;
                    }

                    // Xóa ảnh cũ khỏi Dropbox
                    if (!string.IsNullOrEmpty(oldImageInternalPath))
                    {
                        await _dropboxService.DeleteFileFromDropboxAsync(oldImageInternalPath);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi cập nhật ảnh: {ex.Message}");
                }
            }

            // Lưu thay đổi vào database
            await _trackRepository.UpdateAsync(existingTrack);
        }
        ////hiện tại phương thức này nếu chưa xóa ở dropbox thì sẽ k xóa dc track (ràng buộc)
        //public async Task DeleteAsync(Guid id)
        //{
        //    var trackToDelete = await _trackRepository.GetByIdAsync(id);
        //    if (trackToDelete == null)
        //    {
        //        throw new KeyNotFoundException($"Track with ID {id} not found.");
        //    }

        //    try
        //    {
        //        if (!string.IsNullOrEmpty(trackToDelete.MetaLink))
        //        {
        //            var internalPath = await _dropboxService.GetInternalPathFromSharedLinkAsync(trackToDelete.MetaLink);
        //            if (!string.IsNullOrEmpty(internalPath))
        //            {
        //                var fileDeleted = await _dropboxService.DeleteFileFromDropboxAsync(internalPath);
        //                if (!fileDeleted)
        //                {
        //                    Console.WriteLine($"File not found in Dropbox for track {id}. Proceeding with track deletion.");
        //                }
        //            }
        //        }
        //        if (!string.IsNullOrEmpty(trackToDelete.MetaLinkImage))
        //        {
        //            var imageInternalPath = await _dropboxService.GetInternalPathFromSharedLinkAsync(trackToDelete.MetaLinkImage);
        //            if (!string.IsNullOrEmpty(imageInternalPath))
        //            {
        //                var imageDeleted = await _dropboxService.DeleteFileFromDropboxAsync(imageInternalPath);
        //                if (!imageDeleted)
        //                {
        //                    Console.WriteLine($"Ảnh không tồn tại trên Dropbox cho track {id}. Tiếp tục xóa track.");
        //                }
        //            }
        //        }
        //        await _trackRepository.DeleteAsync(id);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Failed to delete track with ID {id}. Error: {ex.Message}", ex);
        //    }
        //}


        public async Task DeleteAsync(Guid id)
        {
            var trackToDelete = await _trackRepository.GetByIdAsync(id);
            if (trackToDelete == null)
            {
                throw new KeyNotFoundException($"Track with ID {id} not found.");
            }

            try
            {
                // Xử lý file audio
                if (!string.IsNullOrEmpty(trackToDelete.MetaLink))
                {
                    try
                    {
                        var internalPath = await _dropboxService.GetInternalPathFromSharedLinkAsync(trackToDelete.MetaLink);
                        if (!string.IsNullOrEmpty(internalPath))
                        {
                            await _dropboxService.DeleteFileFromDropboxAsync(internalPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi khi xóa file audio: {ex.Message}");
                        // Bỏ qua lỗi và tiếp tục xử lý
                    }
                }

                // Xử lý file ảnh
                if (!string.IsNullOrEmpty(trackToDelete.MetaLinkImage))
                {
                    try
                    {
                        var imageInternalPath = await _dropboxService.GetInternalPathFromSharedLinkAsync(trackToDelete.MetaLinkImage);
                        if (!string.IsNullOrEmpty(imageInternalPath))
                        {
                            await _dropboxService.DeleteFileFromDropboxAsync(imageInternalPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi khi xóa file ảnh: {ex.Message}");
                        // Bỏ qua lỗi và tiếp tục xử lý
                    }
                }

                // Luôn luôn thực hiện xóa track dù có lỗi với file hay không
                await _trackRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete track with ID {id}. Error: {ex.Message}", ex);
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

      

        public async Task IncrementListenCount(Guid trackId)
        {
            await _trackRepository.IncrementListenCount(trackId);
        }

        public async Task<List<Track>> GetRandomTracksAsync()
        {
          return await _trackRepository.GetRandomTracksAsync();
        }
    }
}
