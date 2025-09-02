using AutoMapper;
using Chat.Contexts;
using Chat.Hubs;
using Chat.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Repository
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        private readonly AppDbContext _context;
        public RoleRepository(AppDbContext context, IMapper mapper, IHubContext<ChatHub> hubContext) : base(context, mapper, hubContext)
        {
            _context = context;
        }

        public async Task<IEnumerable<Permission>> GetPermissionsByRoleId(int roleId)
        {
            var role = await _context.Roles
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.Id == roleId);

            return role?.Permissions ?? new List<Permission>();
        }

        public async Task AddPermissionToRole(int roleId, int permissionId)
        {
            var role = await _context.Roles
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.Id == roleId);

            var permission = await _context.Permissions.FindAsync(permissionId);

            if (role != null && permission != null && !role.Permissions.Contains(permission))
            {
                role.Permissions.Add(permission);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemovePermissionFromRole(int roleId, int permissionId)
        {
            var role = await _context.Roles
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.Id == roleId);

            var permission = await _context.Permissions.FindAsync(permissionId);

            if (role != null && permission != null && role.Permissions.Contains(permission))
            {
                role.Permissions.Remove(permission);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateRolePermissions(int roleId, List<int> newPermissionIds)
        {
            var role = await _context.Roles
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.Id == roleId);

            if (role != null)
            {
                role.Permissions.Clear();

                var permissions = await _context.Permissions
                    .Where(p => newPermissionIds.Contains(p.Id))
                    .ToListAsync();

                foreach (var permission in permissions)
                {
                    role.Permissions.Add(permission);
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
