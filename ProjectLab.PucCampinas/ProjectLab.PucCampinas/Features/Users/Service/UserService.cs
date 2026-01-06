using Microsoft.EntityFrameworkCore;
using ProjectLab.PucCampinas.Common.Models;
using ProjectLab.PucCampinas.Common.Services;
using ProjectLab.PucCampinas.Features.Auth.Service;
using ProjectLab.PucCampinas.Features.Users.DTOs;
using ProjectLab.PucCampinas.Features.Users.Model;
using ProjectLab.PucCampinas.Features.Users.Service.shared;
using ProjectLab.PucCampinas.Infrastructure.Data;
using ProjectLab.PucCampinas.Infrastructure.Email.Service;
using ProjectLab.PucCampinas.shared.Service;

namespace ProjectLab.PucCampinas.Features.Users.Service
{
    public class UserService : BaseService, IUserService
    {
        private readonly AppDbContext _context;
        private readonly IViaCepService _viaCepService;
        private readonly AuthService _authService;
        private readonly IEmailService _emailService;

        public UserService(AppDbContext context, ICustomErrorHandler errorHandler, IViaCepService viaCepService)
              : base(errorHandler)
        {
            _context = context;
            _viaCepService = viaCepService;
        }
        public async Task<UserResponse> CreateUser(UserRequest request, AuthService authService)
        {
            try
            {
                var user = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    Ra = request.Ra,
                    Role = request.Role,
                    PhoneNumber = request.PhoneNumber,
                    Cep = request.Cep
                };

                if (!string.IsNullOrWhiteSpace(user.Cep))
                {
                    var address = await _viaCepService.GetAddressByCep(user.Cep);
                    if (address != null)
                    {
                        user.Logradouro = address.Logradouro;
                        user.Bairro = address.Bairro;
                        user.Cidade = address.Localidade;
                        user.Estado = address.Uf;
                    }
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var token = _authService.GenerateSetupToken(user);
                var link = $"http://localhost:4200/setup-password?token={token}";

                var emailData = new
                {
                    name = user.Name,
                    ra = user.Ra, 
                    link = link
                };

                await _emailService.SendTemplateEmail(user.Email, "Defina sua Senha", "Login", emailData);

                return new UserResponse
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    PhoneNumber = user.PhoneNumber,

                    Cep = user.Cep,
                    Logradouro = user.Logradouro,
                    Bairro = user.Bairro,
                    Cidade = user.Cidade,
                    Estado = user.Estado
                };
            }
            catch (Exception ex)
            {
                OnError(ex, 500);
                throw;
            }
        }

        public async Task<UserResponse?> GetUserById(Guid id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null) return null;

                return new UserResponse
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    PhoneNumber = user.PhoneNumber,
                    Cep = user.Cep,
                    Logradouro = user.Logradouro,
                    Bairro = user.Bairro,
                    Cidade = user.Cidade,
                    Estado = user.Estado
                };
            }
            catch (Exception ex)
            {
                OnError(ex, 500);
                throw;
            }
        }

        public async Task DeleteUser(Guid id)
        {
            try
            {
                await _context.Users
                    .Where(u => u.Id == id)
                    .ExecuteDeleteAsync();
            }
            catch (Exception ex)
            {
                OnError(ex, 500);
            }
        }

        public async Task UpdateUser(Guid id, UserRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    throw new Exception("Usuário não encontrado");
                }

                user.Name = request.Name;
                user.Email = request.Email;
                user.Role = request.Role;
                user.PhoneNumber = request.PhoneNumber;
                user.Cep = request.Cep;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                OnError(ex, 500);
            }
        }

        public async Task<PaginatedResult<UserResponse>> SearchUser(SearchUserInput filter)
        {
            try
            {
                var query = _context.Users.AsQueryable();

                if (!string.IsNullOrWhiteSpace(filter.Keyword))
                {
                    query = query.Where(u => u.Name.Contains(filter.Keyword) ||
                                             u.Email.Contains(filter.Keyword) ||
                                             u.PhoneNumber.Contains(filter.Keyword));
                }

                query = filter.Order.ToUpper() == "ASC"
                    ? query.OrderBy(u => u.Name)
                    : query.OrderByDescending(u => u.Name);

                var responseQuery = query.Select(u => new UserResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role,
                    PhoneNumber = u.PhoneNumber,
                    Cep = u.Cep,
                    Logradouro = u.Logradouro,
                    Bairro = u.Bairro,
                    Cidade = u.Cidade,
                    Estado = u.Estado
                });

                return await responseQuery.ToPaginatedResultAsync(filter.Page, filter.Size);
            }
            catch (Exception ex)
            {
                OnError(ex, 500);
                throw;
            }

        }
    }

}
