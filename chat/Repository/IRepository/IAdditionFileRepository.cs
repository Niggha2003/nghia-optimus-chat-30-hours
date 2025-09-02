using Chat.Models;

namespace Chat.Repository.IRepository
{
    public interface IAdditionFileRepository
    {
        Task<List<AdditionFile>> GetFilesByMessageIdAsync(string messageId);
        Task<AdditionFile> GetFileByIdAsync(string id);
        Task CreateAdditionFileAsync(AdditionFile additionFile);
        Task UpdateAdditionFileAsync(AdditionFile additionFile);
        Task DeleteAdditionFileAsync(string id);
    }
}
