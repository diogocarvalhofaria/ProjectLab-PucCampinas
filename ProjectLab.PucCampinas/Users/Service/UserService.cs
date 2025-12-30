using Microsoft.EntityFrameworkCore;
using ProjectLab.PucCampinas.Common.Models;
using ProjectLab.PucCampinas.Data;
using ProjectLab.PucCampinas.shared.Service;
using ProjectLab.PucCampinas.Users.DTOs;
using ProjectLab.PucCampinas.Users.Model;

namespace ProjectLab.PucCampinas.Users.Service
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserById(Guid id)
        {
            return await _context.Users
                .Include(u => u.Reservations)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task DeleteUser(Guid id)
        {
            await _context.Users
                .Where(u => u.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedResult<User>> SearchUser(SearchUserInput filter)
        {
           var entity = _context.Users.AsQueryable();

            if(!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                entity = entity.Where(u => u.Name.Contains(filter.Keyword) || u.Email.Contains(filter.Keyword) || u.PhoneNumber.Contains(filter.Keyword));
            }

            entity = filter.Order.ToUpper() == "ASC"
                ? entity.OrderBy(u => u.Name)
                : entity.OrderByDescending(u => u.Name);

            return await entity.ToPaginatedResultAsync(filter.Page, filter.Size);

        }

    }
}
