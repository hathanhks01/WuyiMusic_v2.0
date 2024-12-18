using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_DAL.IReponsitories
{
    public interface IAdvertisementRepository
    {
        Task<IEnumerable<Advertisement>> GetAllAsync();
        Task<Advertisement> GetByIdAsync(Guid id);
        Task AddAsync(Advertisement advertisement);
        Task UpdateAsync(Advertisement advertisement);
        Task DeleteAsync(Guid id);
    }
}
