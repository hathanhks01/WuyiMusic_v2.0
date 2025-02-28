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
            _context = context;
        }

        public async Task<User> AddUser(User user)
        {
            await _context.Users.AddAsync(user);
            _context.SaveChanges();
            return user;
        }

        public async Task DeleteUser(Guid id)
        {
            var existingUser = await _context.Users.FindAsync(id);

            if (existingUser == null)
            {
                throw new Exception("User not found");
            }
            _context.Remove(existingUser);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllUser()
        {
            var result = await _context.Users.ToListAsync();     
            return result;
        }

        public async Task<User> GetByIdUser(Guid id)
        {
            var result = await _context.Users.FindAsync(id);
            return result;
        }

        public async Task<User> UpdateUser(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.UserId);

            if (existingUser == null)
            {
                throw new Exception("User not found");
            }
            _context.Entry(existingUser).CurrentValues.SetValues(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
