using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<User> AddUser(UserDto userDto)
        {
            return await _userRepo.AddUser(userDto);    
        }

        public Task DeleteUser(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetAllUser()
        {
           return await _userRepo.GetAllUser();
        }

        public async Task<object> GetByIdUser(Guid id)
        {
            return await _userRepo.GetByIdUser(id);
        }

        public async Task<User> UpdateUser(UserDto userDto)
        {
            return await _userRepo.UpdateUser(userDto);
        }
    }
}
