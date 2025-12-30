using Microsoft.EntityFrameworkCore;
using ProjectLab.PucCampinas.Common.Models;
using ProjectLab.PucCampinas.Features.Reservations.DTOs;
using ProjectLab.PucCampinas.Features.Reservations.Model;
using ProjectLab.PucCampinas.Infrastructure.Data;
using ProjectLab.PucCampinas.shared.Service;

namespace ProjectLab.PucCampinas.Features.Reservations.Service
{
    public class ReservationService : IReservationService
    {

        private readonly AppDbContext _context;

        public ReservationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateReservation(Reservation reservation)
        {
            await ValidateConflict(reservation);
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<Reservation?> GetReservationById(Guid id)
        {
            return await _context.Reservations
                .Include(r => r.Laboratory)
                .Include(r => r.User)
                .FirstOrDefaultAsync(reservation => reservation.Id == id);
        }

        public async Task DeleteReservation(Guid id)
        {
            await _context.Reservations
                .Where(reservation => reservation.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task UpdateReservation(Reservation reservation)
        {
            await ValidateConflict(reservation);

            reservation.UpdatedAt = DateTime.Now;
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedResult<Reservation>> SearchReservation(SearchReservationInput filter)
        {
            var entity = _context.Reservations
                .Include(r => r.Laboratory)
                .Include(r => r.User)
                .AsQueryable();

            if(!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                entity = entity.Where(r => r.Laboratory.Name.Contains(filter.Keyword) || r.User.Name.Contains(filter.Keyword));
            }

            if(filter.StartDate.HasValue)
                entity = entity.Where(r => r.ReservationDate >= filter.StartDate.Value);

            if(filter.EndDate.HasValue)
                entity = entity.Where(r => r.ReservationDate <= filter.EndDate.Value);

            entity = filter.Order.ToUpper() == "ASC"
                ? entity.OrderBy(r => r.ReservationDate)
                : entity.OrderByDescending(r => r.ReservationDate);

            return await entity.ToPaginatedResultAsync(filter.Page, filter.Size);

        }

        private async Task ValidateConflict(Reservation reservation)
        {
            var hasConflict = await _context.Reservations.AnyAsync(r =>
                r.Id != reservation.Id && // Aqui está o segredo: ignora a si mesmo no Update
                r.LaboratoryId == reservation.LaboratoryId &&
                r.ReservationDate.Date == reservation.ReservationDate.Date &&
                ((reservation.StartTime >= r.StartTime && reservation.StartTime < r.EndTime) ||
                 (reservation.EndTime > r.StartTime && reservation.EndTime <= r.EndTime)));

            if (hasConflict)
            {
                throw new Exception("O Laboratório já está reservado para este horário!");
            }
        }
    }

}
