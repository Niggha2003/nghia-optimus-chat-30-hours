using AutoMapper;
using Chat.Contexts;
using Chat.Hubs;
using Chat.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Chat.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context, IMapper mapper, IHubContext<ChatHub> hubContext) : base(context, mapper, hubContext) {
            _context = context;
        }

        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public async Task<User?> CheckUserValid(string email, string password) { 
            var user = await _context.Users
                .Where(a => a.UserEmail == email && a.Password == HashPassword(password))
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<IEnumerable<Role>> GetRolesByUserId(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.Roles ?? new List<Role>();
        }

        public async Task AddRoleToUser(int userId, int roleId)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var role = await _context.Roles.FindAsync(roleId);

            if (user != null && role != null && !user.Roles.Contains(role))
            {
                user.Roles.Add(role);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveRoleFromUser(int userId, int roleId)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var role = await _context.Roles.FindAsync(roleId);

            if (user != null && role != null && user.Roles.Contains(role))
            {
                user.Roles.Remove(role);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateUserRoles(int userId, List<int> newRoleIds)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
            {
                user.Roles.Clear();

                var roles = await _context.Roles
                    .Where(r => newRoleIds.Contains(r.Id))
                    .ToListAsync();

                foreach (var role in roles)
                {
                    user.Roles.Add(role);
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
