using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_DAL.Reponsitories
{
    public class AdvertisementRepository : IAdvertisementRepository
    {
        private readonly WuyiMusic_DbContext _context;

        public AdvertisementRepository(WuyiMusic_DbContext context)
        {
            _context = context;
        }

        public async Task<Advertisement> AddAdvertisement(AdvertisementDto advertisementDto)
        {
            var advertisement = new Advertisement
            {
                AdvertisementId = Guid.NewGuid(),
                UserId = advertisementDto.UserId,
                Content = advertisementDto.Content,
            };
            await _context.Advertisements.AddAsync(advertisement);
            _context.SaveChanges();
            return advertisement;
        }

        public Task DeleteAdvertisement(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetAllAdvertisement()
        {
            var result = await _context.Advertisements
        .Include(ad => ad.User)
        .Select(ad => new
        {
            ad.AdvertisementId,
            ad.Content,         
            User = ad.User == null ? null : new
            {
                ad.User.UserId,
                ad.User.Username,
                ad.User.Email,
            }
        }).ToListAsync();
            return result;
        }

        public async Task<object> GetByIdAdvertisement(Guid id)
        {
            var result = await _context.Advertisements.Where(ad => ad.AdvertisementId == id).Select(ad => new
            {
                ad.AdvertisementId,
                ad.Content,
                User = ad.User == null ? null : new
                {
                    ad.User.UserId,
                    ad.User.Username,
                    ad.User.Email,
                }
            }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<Advertisement> UpdateAdvertisement(AdvertisementDto advertisementDto)
        {
            if (advertisementDto == null) throw new ArgumentNullException(nameof(advertisementDto));

            var existingAdvertisement = await _context.Advertisements
                .FirstOrDefaultAsync(ad => ad.AdvertisementId == advertisementDto.AdvertisementId);

            if (existingAdvertisement == null) throw new InvalidOperationException("Advertisement không tồn tại.");

            existingAdvertisement.UserId = advertisementDto.UserId;
            existingAdvertisement.Content = advertisementDto.Content;
            await _context.SaveChangesAsync();
            return existingAdvertisement;
        }
    }

}
