using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_Services.Services
{
    public class UserStatisticsService : IUserStatisticsService
    {
        private readonly IUserStatisticsRepository _repo;
        public UserStatisticsService(IUserStatisticsRepository repo) {
            _repo = repo;
        }
        public async Task<int> GetNewUsersCountByDateAsync(DateTime date) => await _repo.GetNewUsersCountByDateAsync(date);

        public async Task<DailyUserCount[]> GetNewUsersCountForCurrentMonthAsync()
        {
            return await _repo.GetNewUsersCountForCurrentMonthAsync();
        }
    }
}
