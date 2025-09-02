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
    public class GroupController : ControllerBase
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GroupController(IGroupRepository groupRepository, IHttpContextAccessor httpContextAccessor)
        {
            _groupRepository = groupRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: api/Group
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupViewDto>>> GetGroups()
        {
            var groups = await _groupRepository.GetAllAsync();
            var groupViews = _groupRepository.MapList<GroupViewDto>(groups);
            return Ok(groupViews);
        }

        // GET: api/Group/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GroupViewDto>> GetGroup(int id)
        {
            var group = await _groupRepository.GetByIdAsync(id);
            if (group == null)
            {
                return NotFound();
            }
            var groupView = _groupRepository.MapObject<GroupViewDto>(group);
            return Ok(groupView);
        }

        // POST: api/Group
        [HttpPost]
        public async Task<ActionResult<GroupViewDto>> InsertGroup(GroupCreateDto groupCreateDto)
        {
            var groupEntity = _groupRepository.MapObject<Group>(groupCreateDto);
            await _groupRepository.AddAsync(groupEntity);
            await _groupRepository.SaveAsync();
            await _groupRepository.UpdateGroupUsers(groupEntity.Id, groupCreateDto.UserIds);
            var groupView = _groupRepository.MapObject<GroupViewDto>(groupEntity);
            return CreatedAtAction(nameof(GetGroup), new { id = groupEntity.Id }, groupView);
        }

        // PUT: api/Group/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(int id, GroupCreateDto groupCreateDto)
        {
            var groupEntity = _groupRepository.MapObject<Group>(groupCreateDto);
            groupEntity.Id = id;
            await _groupRepository.UpdateAsync(groupEntity);
            return NoContent();
        }

        // DELETE: api/Group/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            await _groupRepository.DeleteAsync(id);
            return NoContent();
        }

        // GET: api/Group/User/{groupId}
        [HttpGet("User/{groupId}")]
        public async Task<ActionResult<IEnumerable<UserViewDto>>> GetGroupUsers(int groupId)
        {
            var users = await _groupRepository.GetUsersByGroupId(groupId);
            if (users == null || users.Count() == 0)
            {
                return NotFound("No users found for this group.");
            }
            var userViews = _groupRepository.MapList<UserViewDto>(users);
            return Ok(userViews);
        }

        // POST: api/Group/User/AddToGroup/{groupId}
        [HttpPost("User/AddToGroup/{groupId}")]
        public async Task<IActionResult> AddUserToGroup(int groupId, [FromBody] int userId)
        {
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
            {
                return NotFound("Group not found.");
            }

            await _groupRepository.AddUserToGroup(groupId, userId);
            return NoContent();
        }

        // DELETE: api/Group/User/RemoveFromGroup/{groupId}
        [HttpDelete("User/RemoveFromGroup/{groupId}")]
        public async Task<IActionResult> RemoveUserFromGroup(int groupId, [FromBody] int userId)
        {
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
            {
                return NotFound("Group not found.");
            }

            await _groupRepository.RemoveUserFromGroup(groupId, userId);
            return NoContent();
        }

        // PUT: api/Group/User/UpdateGroupUsers/{groupId}
        [HttpPut("User/UpdateGroupUsers/{groupId}")]
        public async Task<IActionResult> UpdateGroupUsers(int groupId, [FromBody] List<int> newUserIds)
        {
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
            {
                return NotFound("Group not found.");
            }

            await _groupRepository.UpdateGroupUsers(groupId, newUserIds);
            return NoContent();
        }
    }
}
