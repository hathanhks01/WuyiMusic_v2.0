using Microsoft.AspNetCore.Mvc;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models;


namespace WuyiMusic_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;

        public RoleController(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        // GET: api/role
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetAllRoles()
        {
            var roles = await _roleRepository.GetAllAsync();
            return Ok(roles);
        }

        // GET: api/role/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRoleById(Guid id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return Ok(role);
        }

        // POST: api/role
        [HttpPost]
        public async Task<ActionResult<Role>> CreateRole(Role role)
        {
            await _roleRepository.AddAsync(role);
            return CreatedAtAction(nameof(GetRoleById), new { id = role.RoleId }, role);
        }

        // PUT: api/role/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(Guid id, Role role)
        {
            if (id != role.RoleId)
            {
                return BadRequest();
            }

            await _roleRepository.UpdateAsync(role);
            return NoContent();
        }

        // DELETE: api/role/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            await _roleRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
