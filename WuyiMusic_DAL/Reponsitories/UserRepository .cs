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
    public class UserRepository : IUserRepository
    {
        private readonly WuyiMusic_DbContext _context;
        public UserRepository(WuyiMusic_DbContext context)
        {
            _context=context;
        }

        public async Task<User> AddUser(UserDto userDto)
        {
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = userDto.Username,
                Email = userDto.Email,
                Password = userDto.Password,
                ProfileImage = userDto.ProfileImage,
                IsPremium = userDto.IsPremium,
                CreatedAt = DateTime.Now,
            };
            await _context.Users.AddAsync(user);
            _context.SaveChanges();
            return user;
        }

        public Task DeleteUser(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetAllUser()
        {
            var result = await _context.Users
        .Include(user => user.UserRoles) 
        .ThenInclude(userRole => userRole.Role)
        .Select(user => new
        {
            user.UserId,
            user.Username,
            user.Email,
            Roles = user.UserRoles.Select(userRole => new
            {
                userRole.RoleId,
                userRole.Role.RoleName
            }).ToList()
        })
        .ToListAsync();
            return result;
        }

        public async Task<object> GetByIdUser(Guid id)
        {
            var result = await _context.Users.Where(user => user.UserId == id).Select(user => new
            {
                user.UserId,
                user.Username,
                user.Email,
                Roles = user.UserRoles.Select(userRole => new
                {
                    userRole.RoleId,
                    userRole.Role.RoleName
                }).ToList()
            }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<User> UpdateUser(UserDto userDto)
        {
            if (userDto == null) throw new ArgumentNullException(nameof(userDto));

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(user => user.UserId == userDto.UserId);

            if (existingUser == null) throw new InvalidOperationException("User không tồn tại.");

            existingUser.Username = userDto.Username;
            existingUser.Email = userDto.Email;
            existingUser.Password = userDto.Password;
            existingUser.ProfileImage = userDto.ProfileImage;
            existingUser.IsPremium = userDto.IsPremium;
            existingUser.CreatedAt = userDto.CreatedAt;
            await _context.SaveChangesAsync();
            return existingUser;
        }
    }
}
