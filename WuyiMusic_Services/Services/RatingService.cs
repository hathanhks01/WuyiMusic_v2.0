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
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepo;
        public RatingService(IRatingRepository ratingRepo)
        {
            _ratingRepo = ratingRepo;   
        }

        public Task<Rating> AddRating(RatingDto ratingDto)
        {
            return _ratingRepo.AddRating(ratingDto);
        }

        public Task DeleteRating(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetAllRating()
        {
            return await _ratingRepo.GetAllRating();
        }

        public async Task<object> GetByIdRating(Guid id)
        {
            return await _ratingRepo.GetByIdRating(id);
        }

        public Task<Rating> UpdateRating(RatingDto ratingDto)
        {
            return _ratingRepo.UpdateRating(ratingDto);
        }
    }
}
