using Chat.Models;

namespace Chat.Repository
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<IEnumerable<Permission>> GetPermissionsByRoleId(int roleId);
        Task AddPermissionToRole(int roleId, int permissionId);
        Task RemovePermissionFromRole(int roleId, int permissionId);
        Task UpdateRolePermissions(int roleId, List<int> newPermissionIds);
    }
}
