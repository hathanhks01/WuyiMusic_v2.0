using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_Services.IServices
{
    public interface IUserService
    {
        Task<IEnumerable<object>> GetAllUser();
        Task<object> GetByIdUser(Guid id);
        Task<User> AddUser(UserDto userDto);
        Task<User> UpdateUser(UserDto userDto);
        Task DeleteUser(Guid id);
    }
}
