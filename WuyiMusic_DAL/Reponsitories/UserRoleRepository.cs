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
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly WuyiMusic_DbContext _context;

        public UserRoleRepository(WuyiMusic_DbContext context)
        {
            _context = context;
        }

        public async Task<UserRole> AddUserRole(UserRoleDto userRoleDto)
        {
            var userRoles = new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = userRoleDto.UserId,
                RoleId = userRoleDto.RoleId,
            };
            await _context.UserRoles.AddAsync(userRoles);
            _context.SaveChanges();
            return userRoles;
        }

        public Task DeleteUserRole(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetAllUserRole()
        {
            var result = await _context.UserRoles
                .Include(usrl => usrl.User)
                .Include(usrl => usrl.Role)
                .Select(usrl => new
                {
                    usrl.Id,
                    User = usrl.User == null ? null : new
                    {
                        usrl.UserId,
                        usrl.User.Username,
                        usrl.User.Email
                    },
                    Role = usrl.Role == null ? null : new
                    {
                        usrl.RoleId,
                        usrl.Role.RoleName  
                    }
                }).ToListAsync();

            return result;
        }

        public async Task<object> GetByIdUserRole(Guid id)
        {
            var result = await _context.UserRoles.Where(usrl => usrl.Id == id).Select(usrl => new
            {
                usrl.Id,
                User = usrl.User == null ? null : new
                {
                    usrl.UserId,
                    usrl.User.Username,
                    usrl.User.Email
                },
                Role = usrl.Role == null ? null : new
                {
                    usrl.RoleId,
                    usrl.Role.RoleName
                }
            }).FirstOrDefaultAsync(); ;
            return result;
        }

        public async Task<UserRole> UpdateUserRole(UserRoleDto userRoleDto)
        {
            if (userRoleDto == null) throw new ArgumentNullException(nameof(userRoleDto));

            var existingUserRole = await _context.UserRoles
                .FirstOrDefaultAsync(usrl => usrl.Id == userRoleDto.Id);

            if (existingUserRole == null) throw new InvalidOperationException("UserRole không tồn tại.");

            existingUserRole.UserId = userRoleDto.UserId;
            existingUserRole.RoleId = userRoleDto.RoleId;
            await _context.SaveChangesAsync();
            return existingUserRole;
        }
    }
}
