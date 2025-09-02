using Chat.Contexts;
using Chat.Dtos.ViewDtos;
using Chat.Models;
using Chat.Repository.IRepository;
using Chat.Services.Services;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Chat.Repository.Reposioty
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IMongoCollection<Message> _messageCollection;
        private readonly AppDbContext _context;
        private readonly string _databaseName;

        public MessageRepository(AppDbContext context, IMongoClient mongoClient, IConfiguration configuration)
        {
            _context = context;
            _databaseName = configuration.GetValue<string>("MongoDbSettings:DatabaseName"); 
            var database = mongoClient.GetDatabase(_databaseName);
            _messageCollection = database.GetCollection<Message>("Messages");
        }

        public async Task<List<Message>> GetMessagesOfAGroupAsync(int groupId)
        {
            var filter = Builders<Message>.Filter.And(
                Builders<Message>.Filter.Eq(m => m.ToId, groupId)
            );

            var messages = await _messageCollection
                .Find(filter)
                .SortBy(m => m.CreatedAt)
                .ToListAsync();

            foreach (var message in messages)
            {
                message.Content = EncriptService.DecryptMessage(message.Content);
            }

            return messages;
        }

        public async Task<List<Message>> GetMessagesAsync(int fromId, int toId, bool isGroup)
        {
            var filter = Builders<Message>.Filter.And(
                Builders<Message>.Filter.Eq(m => m.IsGroup, isGroup),
                Builders<Message>.Filter.Or(
                    Builders<Message>.Filter.And(
                        Builders<Message>.Filter.Eq(m => m.FromId, fromId),
                        Builders<Message>.Filter.Eq(m => m.ToId, toId)
                    ),
                    Builders<Message>.Filter.And(
                        Builders<Message>.Filter.Eq(m => m.FromId, toId),
                        Builders<Message>.Filter.Eq(m => m.ToId, fromId)
                    )
                )
            );

            var messages = await _messageCollection
                .Find(filter)
                .SortBy(m => m.CreatedAt)
                .ToListAsync();

            foreach (var message in messages)
            {
                message.Content = EncriptService.DecryptMessage(message.Content);
            }

            return messages;
        }

        public async Task<List<MessageViewDto>> GetMessageByIdAsync(int id)
        {
            var groupIds = _context.Groups
                .Include(g => g.Users)
                .Where(g => g.Users.Any(u => u.Id == id))
                .Select(g => g.Id)
                .ToList();
            var filter = Builders<Message>.Filter.Or(
                    Builders<Message>.Filter.Or(
                        Builders<Message>.Filter.Eq(m => m.FromId, id),
                        Builders<Message>.Filter.Eq(m => m.ToId, id)
                    ),
                    Builders<Message>.Filter.In(m => m.ToId, groupIds)
            );
            
            var messages = await _messageCollection
                .Find(filter)
                .SortBy(m => m.CreatedAt)
                .ToListAsync();

            var groupedMessages = messages
                .GroupBy(m =>
                {
                    // gom nhóm cả 2 chiều: A→B == B→A
                    var minId = Math.Min(m.FromId, m.ToId);
                    var maxId = Math.Max(m.FromId, m.ToId);
                    return (From: minId, To: maxId);
                })
                .Select(g =>
                {
                    var firstMessage = g.OrderByDescending(m => m.CreatedAt).First(); // chỉ order 1 lần
                    var fromUser = _context.Users.FirstOrDefault(u => u.Id == firstMessage.FromId);
                    string toName;

                    if (firstMessage.IsGroup)
                    {
                        var group = _context.Groups.FirstOrDefault(gr => gr.Id == firstMessage.ToId);
                        toName = group?.GroupName ?? "";
                    }
                    else
                    {
                        var toUser = _context.Users.FirstOrDefault(u => u.Id == firstMessage.ToId);
                        toName = toUser?.UserName ?? "";
                    }

                    return new MessageViewDto
                    {
                        Id = firstMessage.Id,
                        FromId = firstMessage.FromId,
                        ToId = firstMessage.ToId,
                        Content = firstMessage.Content,
                        FromName = firstMessage.IsGroup ? toName : (firstMessage.FromId == id ? toName : fromUser?.UserName ?? ""),
                        // đây là tên của người gửi
                        ToName = toName,
                        IsGroup = firstMessage.IsGroup,
                        CreateAt = firstMessage.CreatedAt,
                        IsViewed = firstMessage.IsViewed
                    };
                });

            var userMessages = groupedMessages.Where(m => !m.IsGroup).ToList();
            var groupMessages = groupedMessages
                .Where(m => m.IsGroup)
                .GroupBy(m => m.ToId)
                .Select(g =>
                {
                    var firstMessage = g.OrderByDescending(m => m.CreateAt).First(); // chỉ order 1 lần
                    var fromUser = _context.Users.FirstOrDefault(u => u.Id == firstMessage.FromId);
                    string toName;

                    var group = _context.Groups.FirstOrDefault(gr => gr.Id == firstMessage.ToId);
                    toName = group?.GroupName ?? "";

                    return new MessageViewDto
                    {
                        Id = firstMessage.Id,
                        FromId = firstMessage.FromId,
                        ToId = firstMessage.ToId,
                        Content = firstMessage.Content,
                        FromName = toName,
                        // đây là tên của người gửi
                        ToName = toName,
                        IsGroup = firstMessage.IsGroup,
                        CreateAt = firstMessage.CreateAt,
                        IsViewed = firstMessage.IsViewed
                    };
                }).ToList();

            var result = new List<MessageViewDto>();
            result.AddRange(userMessages);
            result.AddRange(groupMessages);

            foreach (var message in result)
            {
                message.Content = EncriptService.DecryptMessage(message.Content);
            }

            return result;
        }

        public async Task CreateMessageAsync(Message message)
        {
            message.Content = EncriptService.EncryptMessage(message.Content);
            message.CreatedAt = DateTime.UtcNow;

            await _messageCollection.InsertOneAsync(message);
        }

        public async Task UpdateMessageAsync(Message message)
        {
            var filter = Builders<Message>.Filter.Eq(m => m.Id, message.Id);

            message.Content = EncriptService.EncryptMessage(message.Content);

            await _messageCollection.ReplaceOneAsync(filter, message);
        }

        public async Task DeleteMessageAsync(string id)
        {
            var filter = Builders<Message>.Filter.Eq(m => m.Id, id);
            await _messageCollection.DeleteOneAsync(filter);
        }
    }
}





