using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_DAL.Reponsitories
{

    public class UserStatisticsRepository : IUserStatisticsRepository
    {
        private readonly WuyiMusic_DbContext _context;
        public UserStatisticsRepository(WuyiMusic_DbContext context)
        {
            _context = context;
        }
        public async Task<int> GetNewUsersCountByDateAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            return await _context.Set<User>()
                .Where(u => u.CreatedAt >= startDate && u.CreatedAt < endDate)
                .CountAsync();
        }
        public async Task<DailyUserCount[]> GetNewUsersCountForCurrentMonthAsync()
        {
            // Sử dụng DateTime.Today để tránh các vấn đề về giờ (time component)
            DateTime today = DateTime.Today;
            int year = today.Year;
            int month = today.Month;

            // Xác định ngày đầu tiên của tháng và ngày đầu tiên của tháng kế tiếp
            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            DateTime firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);

            // Truy vấn lấy số lượng người dùng mới của từng ngày trong tháng hiện tại
            var counts = await _context.Set<User>()
                .Where(u => u.CreatedAt.HasValue
                            && u.CreatedAt.Value >= firstDayOfMonth
                            && u.CreatedAt.Value < firstDayOfNextMonth)
                .GroupBy(u => u.CreatedAt.Value.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync();

            // Xác định số ngày trong tháng hiện tại
            int daysInMonth = DateTime.DaysInMonth(year, month);
            DailyUserCount[] result = new DailyUserCount[daysInMonth];

            // Lặp qua từng ngày trong tháng, gán số lượng tương ứng (mặc định 0 nếu không có dữ liệu)
            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime currentDate = new DateTime(year, month, day);
                var dataForDate = counts.FirstOrDefault(x => x.Date == currentDate);

                result[day - 1] = new DailyUserCount
                {
                    Date = currentDate,
                    Count = dataForDate?.Count ?? 0
                };
            }

            return result;
        }



    }
}
