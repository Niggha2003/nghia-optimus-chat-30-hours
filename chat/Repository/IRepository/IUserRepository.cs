using Chat.Models;

namespace Chat.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        string HashPassword(string password);
        Task<User?> CheckUserValid(string email, string password);
        Task<IEnumerable<Role>> GetRolesByUserId(int userId);
        Task AddRoleToUser(int userId, int roleId);
        Task RemoveRoleFromUser(int userId, int roleId);
        Task UpdateUserRoles(int userId, List<int> newRoleIds);
    }
}
