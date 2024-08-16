using AutoMapper;
using Contract.DTOs.Users;
using Domain.Entities;

namespace Services.Profiles;

public class RequestProfile : Profile
{
    public RequestProfile()
    {
        #region User

        CreateMap<UserCreateRequest, User>();
        CreateMap<UserUpdateRequest, User>();

        #endregion
    }
}