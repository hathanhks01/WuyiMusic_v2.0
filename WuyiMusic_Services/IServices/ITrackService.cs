using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;
using static WuyiMusic_DAL.Reponsitories.TrackRepository;

namespace WuyiMusic_Services.IServices
{
    public interface ITrackService
    {
        Task<IEnumerable<Track>> GetAllAsync();
        Task<Track> GetByIdAsync(Guid id);
        Task<SearchResultDto> SearchAsync(string searchTerm);
        Task AddTrackAsync(Track track, IFormFile file, IFormFile imageFile);
        Task UpdateAsync(Track updatedTrack, IFormFile file = null, IFormFile imageFile = null);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Track>> GetFavoriteTracksAsync(Guid userId);
        Task<List<Track>> GetTrackRankingByListenCount(DateTime startDate, DateTime endDate, int topCount = 10);
        Task<IEnumerable<Track>> GetRandom6Track(Guid artistId);
        Task IncrementListenCount(Guid trackId);
    }
}
