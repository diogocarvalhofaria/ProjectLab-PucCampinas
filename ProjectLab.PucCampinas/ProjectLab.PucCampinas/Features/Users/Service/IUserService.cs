using ProjectLab.PucCampinas.Common.Models;
using ProjectLab.PucCampinas.Features.Auth.Service;
using ProjectLab.PucCampinas.Features.Users.DTOs;
using ProjectLab.PucCampinas.Features.Users.Model;

namespace ProjectLab.PucCampinas.Features.Users.Service
{
    public interface IUserService
    {
        Task<PaginatedResult<UserResponse>> SearchUser(SearchUserInput filter);
        Task<UserResponse?> GetUserById(Guid id);
        Task<UserResponse> CreateUser(UserRequest request, AuthService authService);
        Task UpdateUser(Guid id, UserRequest request);
        Task DeleteUser(Guid id);
        Task ResendEmail(Guid id);

    }
}
