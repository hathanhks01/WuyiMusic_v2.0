using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_Services.IServices
{
    public interface IUserRoleService
    {
        Task<IEnumerable<object>> GetAllUserRole();
        Task<object> GetByIdUserRole(Guid id);
        Task<UserRole> AddUserRole(UserRoleDto userRoleDto);
        Task<UserRole> UpdateUserRole(UserRoleDto userRoleDto);
        Task DeleteUserRole(Guid id);
    }
}
