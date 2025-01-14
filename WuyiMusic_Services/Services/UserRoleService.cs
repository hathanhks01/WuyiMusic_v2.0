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
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepo;
        public UserRoleService(IUserRoleRepository userRoleRepo)
        {
            _userRoleRepo = userRoleRepo;
        }

        public async Task<UserRole> AddUserRole(UserRoleDto userRoleDto)
        {
            return await _userRoleRepo.AddUserRole(userRoleDto);
        }

        public Task DeleteUserRole(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetAllUserRole()
        {
            return await _userRoleRepo.GetAllUserRole();
        }

        public Task<object> GetByIdUserRole(Guid id)
        {
            return _userRoleRepo.GetByIdUserRole(id);
        }

        public async Task<UserRole> UpdateUserRole(UserRoleDto userRoleDto)
        {
            return await _userRoleRepo.UpdateUserRole(userRoleDto);
        }
    }
}
