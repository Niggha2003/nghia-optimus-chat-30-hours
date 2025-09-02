using Chat.Models;

namespace Chat.Repository
{
    public interface IGroupRepository : IRepository<Group>
    {
        Task<IEnumerable<User>> GetUsersByGroupId(int groupId);
        Task AddUserToGroup(int gorupId, int userId);
        Task RemoveUserFromGroup(int gorupId, int userId);
        Task UpdateGroupUsers(int gorupId, List<int> newuserIds);
    }
}
