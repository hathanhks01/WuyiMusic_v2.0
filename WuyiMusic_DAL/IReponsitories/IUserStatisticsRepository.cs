using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;

namespace WuyiMusic_DAL.IReponsitories
{
    public interface IUserStatisticsRepository
    {
        Task<int> GetNewUsersCountByDateAsync(DateTime date);
        Task<DailyUserCount[]> GetNewUsersCountForCurrentMonthAsync();
    }
}
