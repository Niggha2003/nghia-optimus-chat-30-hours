using AutoMapper;
using Chat.Contexts;
using Chat.Dtos.CreateDtos;
using Chat.Dtos.ViewDtos;
using Chat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using static System.Net.Mime.MediaTypeNames;

namespace Chat.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ChatHub(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = int.Parse(Context.User?.FindFirst("id")?.Value);
            Console.WriteLine($"New connection: {Context.ConnectionId}");

            var newUserSignalR = new UserSignalR {
                UserId = userId,
                UserConnection = Context.ConnectionId
            };

            var currentUserSignalR = _context.UserSignalRs.Where(u => u.UserId == userId).FirstOrDefault();
            if(currentUserSignalR != null)
            {
                currentUserSignalR.UserConnection = Context.ConnectionId;
                _context.UserSignalRs.Update(currentUserSignalR);
            }
            else
            {
                _context.UserSignalRs.Add(newUserSignalR);
            }

            var currentGroupSignalRs = _context.GroupSignalRs.Where(u => u.UserId == userId).ToList();
            foreach (var group in currentGroupSignalRs)
            {
                await Groups.RemoveFromGroupAsync(group.UserConnectionId, group.GroupGuid);

                group.UserConnectionId = Context.ConnectionId;
                await Groups.AddToGroupAsync(Context.ConnectionId, group.GroupGuid);

                _context.GroupSignalRs.Update(group);
            }

            await _context.SaveChangesAsync();
            await Clients.All.SendAsync("UserConnected", userId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = int.Parse(Context.User?.FindFirst("id")?.Value);
            Console.WriteLine($"Connection disconnected: {Context.ConnectionId}");
            await Clients.All.SendAsync("UserDisconnected", userId);
            await base.OnDisconnectedAsync(exception);
        }

        // Client join group
        public async Task JoinGroup(int groupId, List<int> userIds)
        {
            var userId = int.Parse(Context.User?.FindFirst("id")?.Value);
            var deleteGroup = _context.GroupSignalRs.Where(g => g.GroupId == groupId).ToList();
            foreach (var groupUser in deleteGroup) { 
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupUser.GroupGuid);
            }
            _context.GroupSignalRs.RemoveRange(deleteGroup);

            var groupGuidStr = Guid.NewGuid().ToString();

            foreach (var id in userIds)
            {
                var userSignalR = _context.UserSignalRs.Where(u => u.UserId == id).FirstOrDefault();
                var newGroupSignalR = new GroupSignalR
                {
                    GroupId = groupId,
                    UserId = id,
                    UserConnectionId = userSignalR != null? userSignalR.UserConnection : "",
                    GroupGuid = groupGuidStr
                };

                await Groups.AddToGroupAsync(Context.ConnectionId, newGroupSignalR.GroupGuid);
                _context.GroupSignalRs.Add(newGroupSignalR);
            }
            await _context.SaveChangesAsync();
        }

        // Client leave group
        public async Task LeaveGroup(int groupId)
        {
            var userId = int.Parse(Context.User?.FindFirst("id")?.Value);
            var deleteGroupUser = _context.GroupSignalRs.Where(g => g.GroupId == groupId && g.UserId == userId).FirstOrDefault();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, deleteGroupUser.GroupGuid);
            _context.GroupSignalRs.Remove(deleteGroupUser);
            await _context.SaveChangesAsync();
        }

        // Send message to a group
        public async Task SendMessageToGroup(int groupId, MessageViewDto message)
        {
            var userId = int.Parse(Context.User?.FindFirst("id")?.Value);
            var sendGroupUser = _context.GroupSignalRs.Where(g => g.GroupId == groupId).FirstOrDefault();
            var userFrom = _context.Users.Where(u => u.Id == message.FromId).FirstOrDefault();
            var groupTo = _context.Groups.Where(u => u.Id == message.ToId).FirstOrDefault();
            message.FromName = userFrom.UserName;
            message.ToName = groupTo.GroupName;

            if(sendGroupUser != null)
            {
                await Clients.Group(sendGroupUser.GroupGuid).SendAsync("ReceiveMessage", userId, message);
            }
        }

        // Send message
        public async Task SendMessage(int userReceiveId, MessageViewDto message)
        {
            var userFrom = _context.Users.Where(u => u.Id == message.FromId).FirstOrDefault();
            var userTo = _context.Users.Where(u => u.Id == message.ToId).FirstOrDefault();
            message.FromName = userFrom.UserName; 
            message.ToName = userTo.UserName;

            var receiveUserSignalR = _context.UserSignalRs.Where(u => u.UserId == userReceiveId).FirstOrDefault();
            var sendUserSignalR = _context.UserSignalRs.Where(u => u.UserId == message.FromId).FirstOrDefault();

            if ( receiveUserSignalR != null)
            {
                await Clients.Client(receiveUserSignalR.UserConnection).SendAsync("ReceiveMessage", message.FromId, message);
            }
            await Clients.Client(sendUserSignalR.UserConnection).SendAsync("ReceiveMessage", message.FromId, message);
        }

        public async Task ViewedMessage(string messageId)
        {
            await Clients.All.SendAsync("ViewedMessage", messageId);
        }
    }

}
