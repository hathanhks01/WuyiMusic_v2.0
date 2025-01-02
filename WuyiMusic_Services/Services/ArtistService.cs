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
        public ArtistService(IArtistRepository artistRepo)
        {
           _artistRepo = artistRepo;
        }
        public async Task<Artist> AddArtist(ArtistDto artistDto)
        {
            return await _artistRepo.AddArtist(artistDto);
        }

        public Task DeleteArtist(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetAllArtist()
        {
            return await _artistRepo.GetAllArtist();
        }

        public async Task<object> GetByIdArtist(Guid id)
        {
           return await _artistRepo.GetByIdArtist(id);
        }

        public async Task<Artist> UpdateArtist(ArtistDto artistDto)
        {
            return await _artistRepo.UpdateArtist(artistDto);
        }
    }
}
