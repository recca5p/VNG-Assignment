using AutoMapper;
using Contract.DTOs.Books;
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
        CreateMap<UserSignInRequest, User>();

        #endregion
        
        #region Book

        CreateMap<BookCreateRequest, Book>();
        CreateMap<BookUpdateRequest, Book>();

        #endregion
    }
}