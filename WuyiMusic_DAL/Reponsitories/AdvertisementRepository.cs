using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<Advertisement>> GetAllAsync()
        {
            return await _context.Advertisements.ToListAsync();
        }

        public async Task<Advertisement> GetByIdAsync(Guid id)
        {
            return await _context.Advertisements.FindAsync(id);
        }

        public async Task AddAsync(Advertisement advertisement)
        {
            await _context.Advertisements.AddAsync(advertisement);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Advertisement advertisement)
        {
            _context.Advertisements.Update(advertisement);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var advertisement = await GetByIdAsync(id);
            if (advertisement != null)
            {
                _context.Advertisements.Remove(advertisement);
                await _context.SaveChangesAsync();
            }
        }
    }

}
