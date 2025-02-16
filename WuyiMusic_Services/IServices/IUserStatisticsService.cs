using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;

namespace WuyiMusic_Services.IServices
{
    public interface IUserStatisticsService
    {
        Task<int> GetNewUsersCountByDateAsync(DateTime date);
        Task<DailyUserCount[]> GetNewUsersCountForCurrentMonthAsync();
    }
}
