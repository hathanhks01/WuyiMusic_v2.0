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
    public class AdvertisementService : IAdvertisementService
    {
        private readonly IAdvertisementRepository _advertisementRepo;
        public AdvertisementService(IAdvertisementRepository advertisementRepo)
        {
            _advertisementRepo = advertisementRepo;
        }
        public async Task<Advertisement> AddAdvertisement(AdvertisementDto advertisementDto)
        {
            return await _advertisementRepo.AddAdvertisement(advertisementDto);
        }

        public Task DeleteAdvertisement(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetAllAdvertisement()
        {
            return await _advertisementRepo.GetAllAdvertisement();
        }

        public async Task<object> GetByIdAdvertisement(Guid id)
        {
            return await _advertisementRepo.GetByIdAdvertisement(id);
        }

        public async Task<Advertisement> UpdateAdvertisement(AdvertisementDto advertisementDto)
        {
            return await _advertisementRepo.UpdateAdvertisement(advertisementDto);
        }
    }
}
