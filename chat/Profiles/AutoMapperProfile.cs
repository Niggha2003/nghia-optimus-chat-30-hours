using AutoMapper;
using Chat.Dtos.CreateDtos;
using Chat.Dtos.ViewDtos;
using Chat.Models;

namespace Chat.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Ánh xạ từ User (Model) sang UserCreateDto (DTO) 
            CreateMap<User, UserViewDto>();
            // Ánh xạ từ UserCreateDto (DTO) sang User (Model)
            CreateMap<UserCreateDto, User>();

            // Ánh xạ từ Role (Model) sang RoleCreateDto (DTO)
            CreateMap<Role, RoleViewDto>();
            // Ánh xạ từ RoleCreateDto (DTO) sang Role (Model)
            CreateMap<RoleCreateDto, Role>();

            // Ánh xạ từ Permission (Model) sang PermissionCreateDto (DTO)
            CreateMap<Permission, PermissionViewDto>();
            // Ánh xạ từ PermissionCreateDto (DTO) sang Permission (Model)
            CreateMap<PermissionCreateDto, Permission>();

            // Ánh xạ từ Message (Model) sang MessageCreateDto (DTO)
            CreateMap<Message, MessageViewDto>();
            // Ánh xạ từ MessageCreateDto (DTO) sang Message (Model)
            CreateMap<MessageCreateDto, Message>();

            // Ánh xạ từ Group (Model) sang GroupCreateDto (DTO)
            CreateMap<Group, GroupViewDto>();
            // Ánh xạ từ GroupCreateDto (DTO) sang Group (Model)
            CreateMap<GroupCreateDto, Group>();

            // Ánh xạ từ AdditionFile (Model) sang AdditionFileCreateDto (DTO)
            CreateMap<AdditionFile, AdditionFileViewDto>();
            // Ánh xạ từ AdditionFileCreateDto (DTO) sang AdditionFile (Model)
            CreateMap<AdditionFileCreateDto, AdditionFile>();
        }
    }
}
