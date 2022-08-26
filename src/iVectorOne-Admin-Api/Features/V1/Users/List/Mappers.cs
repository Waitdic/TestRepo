namespace iVectorOne_Admin_Api.Features.V1.Users.List
{
    public class Mappers : Profile
    {
        public Mappers()
        {
            CreateMap<User, UserDto>();
        }
    }
}