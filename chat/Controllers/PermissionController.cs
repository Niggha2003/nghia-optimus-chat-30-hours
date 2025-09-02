using Chat.Dtos.CreateDtos;
using Chat.Dtos.ViewDtos;
using Chat.Models;
using Chat.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionController(IPermissionRepository permissionRepository, IHttpContextAccessor httpContextAccessor)
        {
            _permissionRepository = permissionRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: api/Permission
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PermissionViewDto>>> GetPermissions()
        {
            var permissions = await _permissionRepository.GetAllAsync();
            var permissionViews = _permissionRepository.MapList<PermissionViewDto>(permissions);
            return Ok(permissionViews);
        }


        // GET: api/Permission/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PermissionViewDto>> GetPermission(int id)
        {
            var permission = await _permissionRepository.GetByIdAsync(id);
            if (permission == null)
            {
                return NotFound();
            }
            var permissionView = _permissionRepository.MapObject<PermissionViewDto>(permission);
            return Ok(permissionView);
        }

        // POST: api/Permission
        [HttpPost]
        public async Task<ActionResult<PermissionViewDto>> InsertPermission(PermissionCreateDto permissionCreateDto)
        {
            var permissionEntity = _permissionRepository.MapObject<Permission>(permissionCreateDto);
            await _permissionRepository.AddAsync(permissionEntity);
            await _permissionRepository.SaveAsync();
            var permissionView = _permissionRepository.MapObject<PermissionViewDto>(permissionEntity);
            return CreatedAtAction(nameof(GetPermission), new { id = permissionEntity.Id }, permissionView);
        }

        // PUT: api/Permission/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePermission(int id, PermissionCreateDto permissionCreateDto)
        {
            var permissionEntity = _permissionRepository.MapObject<Permission>(permissionCreateDto);
            permissionEntity.Id = id;
            await _permissionRepository.UpdateAsync(permissionEntity);
            return NoContent();
        }

        // DELETE: api/Permission/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            await _permissionRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
