using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_Services.IServices
{
    public interface IRatingService
    {
        Task<IEnumerable<object>> GetAllRating();
        Task<object> GetByIdRating(Guid id);
        Task<Rating> AddRating(RatingDto ratingDto);
        Task<Rating> UpdateRating(RatingDto ratingDto);
        Task DeleteRating(Guid id);
    }
}
