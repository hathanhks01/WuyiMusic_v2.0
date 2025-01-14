using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_Services.IServices
{
    public interface IRoleService
    {
        Task<IEnumerable<object>> GetAllRole();
        Task<object> GetByIdRole(Guid id);
        Task<Role> AddRole(RoleDto roleDto);
        Task<Role> UpdateRole(RoleDto roleDto);
        Task DeleteRole(Guid id);
    }
}
