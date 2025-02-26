using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_Services.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IArtistRepository _artistRepo;
        private readonly IDropboxService _dropboxService;
        public ArtistService(IArtistRepository artistRepo, IDropboxService dropboxService)
        {
           _artistRepo = artistRepo;
           _dropboxService = dropboxService;
        }
        public async Task<Artist> AddArtist(ArtistDto artistDto,Guid userId)
        {
            artistDto.ArtistId = userId;
            string imageUrl = null;
            string metalink=null;
            if (artistDto.ArtistImageFile != null)
            {
                string fileName = $"{artistDto.ArtistId}_{Guid.NewGuid()}{Path.GetExtension(artistDto.ArtistImageFile.FileName)}";

                try
                {
                    using (var imageStream = artistDto.ArtistImageFile.OpenReadStream())
                    {
                        // Verify stream is not empty
                        if (imageStream.Length == 0)
                        {
                            throw new Exception("Image stream is empty");
                        }

                        var dropboxFileInfo = await _dropboxService.UploadImageAsync(imageStream, fileName);

                        // Verify dropbox upload succeeded
                        if (dropboxFileInfo == null)
                        {
                            throw new Exception("Dropbox upload failed");
                        }
                        var link = await _dropboxService.GetPermanentSharedLinkImageAsync(fileName);
                        imageUrl =link.DirectLink;
                        metalink =link.SharedLink;
                    }

                    artistDto.ArtistImage = imageUrl;
                    artistDto.MetaLink = metalink;
                }
                catch (Exception ex)
                {
                    // Log the full exception details
                    Console.WriteLine($"Artist image upload error: {ex}");
                    // Optionally set a default image or rethrow
                    throw;
                }
            }

            return await _artistRepo.AddArtist(artistDto,userId);
        }
        public Task DeleteArtist(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetAllArtist()
        {
            return await _artistRepo.GetAllArtist();
        }

        public async Task<Artist?> GetArtistByUserIdAsync(Guid userId)
        {
            return await _artistRepo.GetArtistByUserIdAsync(userId);
        }

        public async Task<object> GetByIdArtist(Guid id)
        {
           return await _artistRepo.GetByIdArtist(id);
        }

        public async Task<Artist> UpdateArtist(ArtistDto artistDto)
        {
            if (artistDto == null) throw new ArgumentNullException(nameof(artistDto));

            // Lấy artist hiện tại từ database
            var existingArtist = await _artistRepo.GetByIdArtist(artistDto.ArtistId) as Artist;
            if (existingArtist == null)
                throw new KeyNotFoundException($"Không tìm thấy Artist với ID: {artistDto.ArtistId}");

            string oldMetaLink = null;
            existingArtist.Name = artistDto.Name; 
            existingArtist.Bio = artistDto.Bio; 

            if (artistDto.ArtistImageFile == null || artistDto.ArtistImageFile.Length == 0)
            {
                artistDto.ArtistImage = existingArtist.ArtistImage;
                artistDto.MetaLink = existingArtist.MetaLink;
            }
            if (artistDto.ArtistImageFile != null && artistDto.ArtistImageFile.Length > 0)
            {
                try
                {
                    string fileName = $"{artistDto.ArtistId}_{Guid.NewGuid()}{Path.GetExtension(artistDto.ArtistImageFile.FileName)}";
                    oldMetaLink = existingArtist.MetaLink;

                    using (var imageStream = artistDto.ArtistImageFile.OpenReadStream())
                    {
                        var dropboxFileInfo = await _dropboxService.UploadImageAsync(imageStream, fileName);
                        var link = await _dropboxService.GetPermanentSharedLinkImageAsync(fileName);
                        existingArtist.ArtistImage = link.DirectLink; 
                        existingArtist.MetaLink = link.SharedLink;     
                    }

                    if (!string.IsNullOrEmpty(oldMetaLink))
                    {
                        var oldFilePath = await _dropboxService.GetInternalPathFromSharedLinkAsync(oldMetaLink);
                        await _dropboxService.DeleteFileFromDropboxAsync(oldFilePath);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi cập nhật ảnh Artist: {ex}");
                    throw new Exception("Không thể cập nhật ảnh cho Artist", ex);
                }
            }
            return await _artistRepo.UpdateArtist(existingArtist); 
        }
        public async Task<IEnumerable<Artist>> GetRandomArtistsAsync()
        {
          return await _artistRepo.GetRandomArtistsAsync();
        }

        public async Task<Artist> AddArtistForAdm(ArtistDto artistDto)
        {
            string imageUrl = null;
            string metalink = null;
            var artist = new Artist
            {
                ArtistId = Guid.NewGuid(),
                Name = artistDto.Name,
                Bio = artistDto.Bio,
                IsVerified = artistDto.IsVerified,
                CreatedAt=DateTime.Now,
            };

            if (artistDto.ArtistImageFile != null)
            {
                string fileName = $"{artistDto.ArtistId}_{Guid.NewGuid()}{Path.GetExtension(artistDto.ArtistImageFile.FileName)}";

                try
                {
                    using (var imageStream = artistDto.ArtistImageFile.OpenReadStream())
                    {
                        if (imageStream.Length == 0)
                        {
                            throw new Exception("Image stream is empty");
                        }

                        var dropboxFileInfo = await _dropboxService.UploadImageAsync(imageStream, fileName);

                        if (dropboxFileInfo == null)
                        {
                            throw new Exception("Dropbox upload failed");
                        }
                        var link = await _dropboxService.GetPermanentSharedLinkImageAsync(fileName);
                        imageUrl = link.DirectLink;
                        metalink = link.SharedLink;
                    }

                    artist.ArtistImage = imageUrl;
                    artist.MetaLink = metalink;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Artist image upload error: {ex}");
                    throw;
                }
            }
            return await _artistRepo.AddArtistForAdm(artist);
        }

    }
}
