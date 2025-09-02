using Chat.Dtos.CreateDtos;
using Chat.Dtos.ViewDtos;
using Chat.Models;
using Chat.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RoleController(IRoleRepository roleRepository, IHttpContextAccessor httpContextAccessor)
        {
            _roleRepository = roleRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: api/Role
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleViewDto>>> GetRoles()
        {
            var roles = await _roleRepository.GetAllAsync();
            var roleViews = _roleRepository.MapList<RoleViewDto>(roles);
            return Ok(roleViews);
        }

        // GET: api/Role/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleViewDto>> GetRole(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            var roleView = _roleRepository.MapObject<RoleViewDto>(role);

            return Ok(roleView);
        }

        // POST: api/Role
        [HttpPost]
        public async Task<ActionResult<RoleViewDto>> InsertRole(RoleCreateDto role)
        {
            var roleEntity = _roleRepository.MapObject<Role>(role);
            await _roleRepository.AddAsync(roleEntity);
            await _roleRepository.SaveAsync();
            var roleView = _roleRepository.MapObject<RoleViewDto>(roleEntity);
            return CreatedAtAction(nameof(GetRole), new { id = roleEntity.Id }, roleView);
        }

        // PUT: api/Role/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, RoleCreateDto role)
        {
            var roleEntity = _roleRepository.MapObject<Role>(role);
            roleEntity.Id = id;
            await _roleRepository.UpdateAsync(roleEntity);
            return NoContent();
        }

        // DELETE: api/Role/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            await _roleRepository.DeleteAsync(id);
            return NoContent();
        }

        // GET: api/Role/Permissions/{roleId}
        [HttpGet("Permissions/{roleId}")]
        public async Task<ActionResult<IEnumerable<PermissionViewDto>>> GetPermissionsByRoleId(int roleId)
        {
            var permissions = await _roleRepository.GetPermissionsByRoleId(roleId);

            if (permissions == null || permissions.Count() == 0)
            {
                return NotFound("No permissions found for this role.");
            }
            var permissionViews = _roleRepository.MapList<PermissionViewDto>(permissions);
            return Ok(permissionViews);
        }

        // POST: api/Role/Permissions/AddPermissionToRole/{roleId}
        [HttpPost("Permissions/AddPermissionToRole/{roleId}")]
        public async Task<IActionResult> AddPermissionToRole(int roleId, [FromBody] int permissionId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                return NotFound("Role not found.");
            }

            await _roleRepository.AddPermissionToRole(roleId, permissionId);
            return NoContent();
        }

        // DELETE: api/Role/Permissions/RemovePermissionFromRole/{roleId}
        [HttpDelete("Permissions/RemovePermissionFromRole/{roleId}")]
        public async Task<IActionResult> RemovePermissionFromRole(int roleId, [FromBody] int permissionId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                return NotFound("Role not found.");
            }

            await _roleRepository.RemovePermissionFromRole(roleId, permissionId);
            return NoContent();
        }

        // PUT: api/Role/Permissions/UpdateRolePermissions/{roleId}
        [HttpPut("Permissions/UpdateRolePermissions/{roleId}")]
        public async Task<IActionResult> UpdateRolePermissions(int roleId, [FromBody] List<int> newPermissionIds)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                return NotFound("Role not found.");
            }

            await _roleRepository.UpdateRolePermissions(roleId, newPermissionIds);
            return NoContent();
        }
    }
}
