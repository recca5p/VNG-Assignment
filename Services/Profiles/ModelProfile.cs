using AutoMapper;
using Contract.DTOs.Books;
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

        #region Book

        CreateMap<Book, BookModel>()
            .ForMember(dest => dest.PublishYear, opt => opt.MapFrom(src => src.PublishYear.Year))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty));
        #endregion
    }
}