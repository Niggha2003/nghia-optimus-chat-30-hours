using Chat.Dtos.ViewDtos;
using Chat.Models;

namespace Chat.Repository.IRepository
{
    public interface IMessageRepository
    {
        Task<List<Message>> GetMessagesOfAGroupAsync(int groupId);
        Task<List<Message>> GetMessagesAsync(int fromId, int toId, bool isGroup);
        Task<List<MessageViewDto>> GetMessageByIdAsync(int id);
        Task CreateMessageAsync(Message message);
        Task UpdateMessageAsync(Message message);
        Task DeleteMessageAsync(string id);
    }
}
