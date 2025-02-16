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
        public async Task<Artist> AddArtist(ArtistDto artistDto, Guid userId)
        {
            string imageUrl = null;
            string metalink=null;
            if (artistDto.ArtistImageFile != null)
            {
                string fileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(artistDto.ArtistImageFile.FileName)}";

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

            return await _artistRepo.AddArtist(artistDto, userId);
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
            return await _artistRepo.UpdateArtist(artistDto);
        }
       public async Task<IEnumerable<Artist>> GetRandomArtistsAsync()
        {
          return await _artistRepo.GetRandomArtistsAsync();
        }
    }
}
