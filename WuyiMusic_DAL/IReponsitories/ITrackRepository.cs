using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;
using static WuyiMusic_DAL.Reponsitories.TrackRepository;

namespace WuyiMusic_DAL.IReponsitories
{
    public interface ITrackRepository
    {
        Task<IEnumerable<Track>> GetAllAsync();
        Task<Track> GetByIdAsync(Guid id);
        Task<SearchResultDto> SearchAsync(string searchTerm);
        Task AddAsync(Track track, IFormFile file);
        Task UpdateAsync(Track track);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Track>> GetFavoriteTracksAsync(Guid userId);
        Task<List<Track>> GetTrackRankingByListenCount(DateTime startDate, DateTime endDate, int topCount = 10);
        Task<IEnumerable<Track>> GetRandom6Track(Guid artistId);
        Task IncrementListenCount(Guid trackId);
    }
}
