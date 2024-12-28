using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_DAL.IReponsitories
{
    public interface IAdvertisementRepository
    {
        Task<IEnumerable<object>> GetAllAdvertisement();
        Task<object> GetByIdAdvertisement(Guid id);
        Task<Advertisement> AddLyrics(AdvertisementDto advertisementDto);
        Task<Advertisement> UpdateAdvertisement(AdvertisementDto advertisementDto);
        Task DeleteAdvertisement(Guid id);
    }
}
