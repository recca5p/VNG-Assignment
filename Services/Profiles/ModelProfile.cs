using AutoMapper;
using Contract.DTOs.Users;
using Domain.Entities;

namespace Services.Profiles;

public class ModelProfile : Profile
{
    public ModelProfile()
    {
        #region User

        CreateMap<User, UserModel>();

        #endregion
    }
}