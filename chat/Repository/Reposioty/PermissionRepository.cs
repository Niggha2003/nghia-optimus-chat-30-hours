using AutoMapper;
using Chat.Contexts;
using Chat.Hubs;
using Chat.Models;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Repository
{
    public class PermissionRepository : Repository<Permission>, IPermissionRepository
    {
        public PermissionRepository(AppDbContext context, IMapper mapper, IHubContext<ChatHub> hubContext) : base(context, mapper, hubContext) { }

    }
}
