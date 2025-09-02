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
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: api/User/CurrentUser
        [HttpGet("CurrentUser")]
        public async Task<ActionResult<UserViewDto>> GetCurrentUser()
        {
            int currentUserId = 0;
            if (_httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value != null)
            {
                currentUserId = int.Parse(_httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value);
            }
            var user = await _userRepository.GetByIdAsync(currentUserId);
            if (user == null)
            {
                return NotFound();
            }
            var userView = _userRepository.MapObject<UserViewDto>(user);
            var roles = await _userRepository.GetRolesByUserId(user.Id);
            userView.RoleName = string.Join(", ", roles.Select(r => r.RoleName).ToList());
            return Ok(userView);
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserViewDto>>> GetUsers()
        {
            var users = await _userRepository.GetAllAsync();
            var userViews = _userRepository.MapList<UserViewDto>(users);
            var userViewsToList = userViews.ToList();
            if (users != null)
            {
                for (int i = 0; i < users.Count(); i++)
                {
                    var roles = await _userRepository.GetRolesByUserId(users.ToList()[i].Id);
                    userViewsToList[i].RoleName = string.Join(", ", roles.Select(r => r.RoleName).ToList());
                }
            }
            return Ok(userViewsToList);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserViewDto>> GetUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var userView = _userRepository.MapObject<UserViewDto>(user);
            var roles = await _userRepository.GetRolesByUserId(user.Id);
            userView.RoleName = string.Join(", ", roles.Select(r => r.RoleName).ToList());
            return Ok(userView);
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<UserViewDto>> InsertUser(UserCreateDto user)
        {
            var userEntity = _userRepository.MapObject<User>(user);
            userEntity.Password = _userRepository.HashPassword(userEntity.Password);
            await _userRepository.AddAsync(userEntity);
            await _userRepository.SaveAsync();
            await _userRepository.UpdateUserRoles(userEntity.Id, user.RoleIds);
            var userView = _userRepository.MapObject<UserViewDto>(userEntity);
            return CreatedAtAction(nameof(GetUser), new { id = userEntity.Id }, userView);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserCreateDto user)
        {
            var userEntity = _userRepository.MapObject<User>(user);
            userEntity.Id = id;
            await _userRepository.UpdateAsync(userEntity);
            return NoContent();
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userRepository.DeleteAsync(id);
            return NoContent();
        }

        // GET: api/User/Role/{userId}
        [HttpGet("Role/{userId}")]
        public async Task<ActionResult<IEnumerable<RoleViewDto>>> GetUserRoles(int userId)
        {
            var roles = await _userRepository.GetRolesByUserId(userId);
            if (roles == null || roles.Count() == 0)
            {
                return NotFound("No roles found for this user.");
            }
            var roleViews = _userRepository.MapList<RoleViewDto>(roles);
            return Ok(roleViews);
        }

        // POST: api/User/Role/AddToUser/{userId}
        [HttpPost("Role/AddToUser/{userId}")]
        public async Task<IActionResult> AddRoleToUser(int userId, [FromBody] int roleId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            await _userRepository.AddRoleToUser(userId, roleId);
            return NoContent();
        }

        // DELETE: api/User/Role/RemoveFromUser/{userId}
        [HttpDelete("Role/RemoveFromUser/{userId}")]
        public async Task<IActionResult> RemoveRoleFromUser(int userId, [FromBody] int roleId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            await _userRepository.RemoveRoleFromUser(userId, roleId);
            return NoContent();
        }

        // PUT: api/User/Role/UpdateUserRoles/{userId}
        [HttpPut("Role/UpdateUserRoles/{userId}")]
        public async Task<IActionResult> UpdateUserRoles(int userId, [FromBody] List<int> newRoleIds)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            await _userRepository.UpdateUserRoles(userId, newRoleIds);
            return NoContent();
        }
    }
}
