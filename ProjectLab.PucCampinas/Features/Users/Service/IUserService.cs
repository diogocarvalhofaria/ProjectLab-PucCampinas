using ProjectLab.PucCampinas.Common.Models;
using ProjectLab.PucCampinas.Features.Users.DTOs;
using ProjectLab.PucCampinas.Features.Users.Model;

namespace ProjectLab.PucCampinas.Features.Users.Service
{
    public interface IUserService
    {
        Task<PaginatedResult<User>> SearchUser(SearchUserInput filter);
        Task<User?> GetUserById(Guid id);
        Task CreateUser(User user);
        Task UpdateUser(User user);
        Task DeleteUser(Guid id);

    }
}
