using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;
using WuyiMusic_DAL.IReponsitories;
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
            // Mapping từ UserDto sang User
            var user = new User
            {
                UserId = userDto.UserId == Guid.Empty ? Guid.NewGuid() : userDto.UserId,
                Username = userDto.Username,
                Email = userDto.Email,
                Password = userDto.Password,
                ProfileImage = userDto.ProfileImage,
                CreatedAt = userDto.CreatedAt ?? DateTime.UtcNow,
                IsPremium = userDto.IsPremium
            };

            return await _userRepo.AddUser(user);
        }

        public async Task<User> UpdateUser(UserDto userDto)
        {
            // Mapping từ UserDto sang User
            var user = new User
            {
                UserId = userDto.UserId,
                Username = userDto.Username,
                Email = userDto.Email,
                Password = userDto.Password,
                ProfileImage = userDto.ProfileImage,
                CreatedAt = userDto.CreatedAt,
                IsPremium = userDto.IsPremium
            };

            return await _userRepo.UpdateUser(user);
        }

        public async Task<IEnumerable<User>> GetAllUser()
        {
            return await _userRepo.GetAllUser();
        }

        public async Task<User> GetByIdUser(Guid id)
        {
            return await _userRepo.GetByIdUser(id);
        }

        public async Task DeleteUser(Guid id)
        {
            await _userRepo.DeleteUser(id);
        }
    }
}
