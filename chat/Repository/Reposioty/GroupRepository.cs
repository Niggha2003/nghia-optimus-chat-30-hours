using AutoMapper;
using Chat.Contexts;
using Chat.Hubs;
using Chat.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Chat.Repository
{
    public class GroupRepository : Repository<Group>, IGroupRepository
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public GroupRepository(AppDbContext context, IMapper mapper, IHubContext<ChatHub> hubContext) : base(context, mapper, hubContext) {
            _context = context;
            _hubContext = hubContext;
        }


        public async Task<IEnumerable<User>> GetUsersByGroupId(int groupId)
        {
            var group = await _context.Groups
                .Include(u => u.Users)
                .FirstOrDefaultAsync(u => u.Id == groupId);

            return group?.Users ?? new List<User>();
        }

        public async Task AddUserToGroup(int groupId, int userId)
        {
            var group = await _context.Groups
                .Include(u => u.Users)
                .FirstOrDefaultAsync(u => u.Id == groupId);

            var user = await _context.Users.FindAsync(userId);

            if (group != null && user != null && !group.Users.Contains(user))
            {
                group.Users.Add(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveUserFromGroup(int groupId, int userId)
        {
            var group = await _context.Groups
                .Include(u => u.Users)
                .FirstOrDefaultAsync(u => u.Id == groupId);

            var user = await _context.Users.FindAsync(userId);

            if (group != null && user != null && group.Users.Contains(user))
            {
                group.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateGroupUsers(int groupId, List<int> newUserIds)
        {
            var group = await _context.Groups
                .Include(u => u.Users)
                .FirstOrDefaultAsync(u => u.Id == groupId);

            if (group != null)
            {
                group.Users.Clear();

                var users = await _context.Users
                    .Where(r => newUserIds.Contains(r.Id))
                    .ToListAsync();

                foreach (var user in users)
                {
                    group.Users.Add(user);
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
