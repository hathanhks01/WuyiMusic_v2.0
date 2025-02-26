using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models;
using WuyiMusic_DAL.Reponsitories;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_Services.Services
{
    public class AlbumService : IAlbumService
    {
        private readonly IAlbumRepository _albumRepo;
        private readonly IDropboxService _dropboxService;
        private readonly ITrackRepository _trackRepository;
        private readonly ILogger<AlbumService> _logger;
        public AlbumService(IAlbumRepository albumRepo, IDropboxService dropboxService, ITrackRepository trackRepository, ILogger<AlbumService> logger) 
        { 
            _albumRepo = albumRepo;
            _dropboxService = dropboxService;
            _trackRepository = trackRepository;
            _logger = logger;
        }

        private string GetFullExceptionMessage(Exception ex)
        {
            var messages = new StringBuilder();
            var currentEx = ex;
            var indent = "";

            while (currentEx != null)
            {
                messages.AppendLine($"{indent}Error: {currentEx.Message}");
                messages.AppendLine($"{indent}Stack Trace: {currentEx.StackTrace}");

                if (currentEx is SqlException sqlEx)
                {
                    messages.AppendLine($"{indent}SQL Error Number: {sqlEx.Number}");
                    messages.AppendLine($"{indent}SQL State: {sqlEx.State}");
                }

                indent += "    ";
                currentEx = currentEx.InnerException;
            }

            return messages.ToString();
        }

        public async Task<Album> AddAlbum(Album album, IFormFile imageFile, Dictionary<int, IFormFile> trackFiles)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                var error = "File ảnh album không hợp lệ";
                _logger.LogError(error);
                throw new ArgumentNullException(nameof(imageFile), error);
            }

            try
            {
                // Upload album image first
                try
                {
                    using (var imageStream = imageFile.OpenReadStream())
                    {
                        await _dropboxService.UploadImageAsync(imageStream, imageFile.FileName);
                        var imgFile = await _dropboxService.GetPermanentSharedLinkImageAsync(imageFile.FileName);
                        album.albumImage = imgFile.DirectLink;
                        album.MetaLinkImage = imgFile.SharedLink;
                    }
                }
                catch (Exception ex)
                {
                    var error = $"Lỗi khi upload ảnh album: {GetFullExceptionMessage(ex)}";
                    _logger.LogError(error);
                    throw new Exception(error, ex);
                }

                // Create the album
                Album createdAlbum;
                try
                {
                    createdAlbum = await _albumRepo.AddAlbum(album);
                    _logger.LogInformation($"Đã tạo album mới: {album.Title} với ID: {album.AlbumId}");
                }
                catch (Exception ex)
                {
                    var error = $"Lỗi khi tạo album trong database: {GetFullExceptionMessage(ex)}";
                    _logger.LogError(error);
                    throw new Exception(error, ex);
                }

                if (createdAlbum != null && album.Tracks != null && album.Tracks.Any())
                {
                    for (int i = 0; i < album.Tracks.Count; i++)
                    {
                        var track = album.Tracks.ElementAt(i);
                        try
                        {
                            // Set basic track properties
                            track.TrackId = Guid.NewGuid();
                            track.AlbumId = createdAlbum.AlbumId;
                            track.TrackImage = createdAlbum.albumImage;
                            track.MetaLinkImage = createdAlbum.MetaLinkImage;
                            track.ArtistId = track.ArtistId ?? createdAlbum.ArtistId;
                            track.CreatedAt = DateTime.UtcNow;
                            track.ListenCount = track.ListenCount ?? 0;

                            _logger.LogInformation($"Đang xử lý track: {track.Title} với ID: {track.TrackId}");

                            if (trackFiles.TryGetValue(i, out var trackFile))
                            {
                                if (trackFile == null || trackFile.Length == 0)
                                {
                                    var error = $"File nhạc không hợp lệ cho track: {track.Title}";
                                    _logger.LogError(error);
                                    throw new ArgumentException(error);
                                }

                                try
                                {
                                    using (var stream = trackFile.OpenReadStream())
                                    {
                                        await _dropboxService.UploadFileAsync(stream, trackFile.FileName);
                                        var sharedLink = await _dropboxService.GetPermanentSharedLinkAsync(trackFile.FileName);
                                        track.FilePath = sharedLink.DirectLink;
                                        track.MetaLink = sharedLink.SharedLink;
                                    }

                                    await _trackRepository.AddAsync(track, trackFile);
                                    _logger.LogInformation($"Đã thêm thành công track: {track.Title}");
                                }
                                catch (Exception ex)
                                {
                                    var error = $"Lỗi khi xử lý track {track.Title}: {GetFullExceptionMessage(ex)}";
                                    _logger.LogError(error);
                                    throw new Exception(error, ex);
                                }
                            }
                            else
                            {
                                var error = $"Không tìm thấy file nhạc cho track: {track.Title}";
                                _logger.LogError(error);
                                throw new ArgumentException(error);
                            }
                        }
                        catch (Exception ex)
                        {
                            var error = $"Lỗi khi xử lý track {track.Title}: {GetFullExceptionMessage(ex)}";
                            _logger.LogError(error);
                            throw new Exception(error, ex);
                        }
                    }
                }

                return createdAlbum;
            }
            catch (Exception ex)
            {
                var error = $"Lỗi tổng thể khi thêm album: {GetFullExceptionMessage(ex)}";
                _logger.LogError(error);
                throw new Exception(error, ex);
            }
        }

        public async Task DeleteAlbum(Guid id)
        {
           await _albumRepo.DeleteAlbum(id);
        }

        public async Task<IEnumerable<object>> GetAllAlbum()
        {
            return await _albumRepo.GetAllAlbum();
        }

        public async Task<object> GetByIdAlbum(Guid id)
        {
            return await _albumRepo.GetByIdAlbum(id);
        }

        public async Task<Album> UpdateAlbum(AlbumDto albumDto)
        {
            return await _albumRepo.UpdateAlbum(albumDto);
        }
    }
}
