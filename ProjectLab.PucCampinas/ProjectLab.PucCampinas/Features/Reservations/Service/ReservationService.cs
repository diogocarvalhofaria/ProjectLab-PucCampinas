using Microsoft.EntityFrameworkCore;
using ProjectLab.PucCampinas.Common.Models;
using ProjectLab.PucCampinas.Common.Services;
using ProjectLab.PucCampinas.Features.Reservations.DTOs;
using ProjectLab.PucCampinas.Features.Reservations.Model;
using ProjectLab.PucCampinas.Infrastructure.Data;
using ProjectLab.PucCampinas.shared.Service;

namespace ProjectLab.PucCampinas.Features.Reservations.Service
{
    public class ReservationService : BaseService, IReservationService
    {

        private readonly AppDbContext _context;

        public ReservationService(AppDbContext context, ICustomErrorHandler errorHandler)
             : base(errorHandler)
        {
            _context = context;
        }

        public async Task<ReservationResponse> CreateReservation(ReservationRequest request)
        {
            try
            {
                var reservation = new Reservation
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    LaboratoryId = request.LaboratoryId,
                    ReservationDate = request.ReservationDate,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime
                };

                await ValidateConflict(reservation);

                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();

                var user = await _context.Users.FindAsync(reservation.UserId);
                var lab = await _context.Laboratories.FindAsync(reservation.LaboratoryId);

                return new ReservationResponse
                {
                    Id = reservation.Id,
                    ReservationDate = reservation.ReservationDate,
                    StartTime = reservation.StartTime,
                    EndTime = reservation.EndTime,
                    UserId = reservation.UserId,
                    UserName = user?.Name ?? "Desconhecido",
                    LaboratoryId = reservation.LaboratoryId,
                    LaboratoryName = lab?.Name ?? "Desconhecido"
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao criar reserva: " + ex.Message);
            }
        }

        public async Task<ReservationResponse?> GetReservationById(Guid id)
        {
            try
            {
                var r = await _context.Reservations
                    .Include(x => x.Laboratory)
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (r == null) return null;

                return new ReservationResponse
                {
                    Id = r.Id,
                    ReservationDate = r.ReservationDate,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime,
                    UserId = r.UserId,
                    UserName = r.User?.Name ?? string.Empty,
                    LaboratoryId = r.LaboratoryId,
                    LaboratoryName = r.Laboratory?.Name ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter reserva: " + ex.Message);
            }
        }

        public async Task DeleteReservation(Guid id)
        {
            try
            {
                await _context.Reservations
                    .Where(reservation => reservation.Id == id)
                    .ExecuteDeleteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao deletar reserva: " + ex.Message);
            }
        }

        public async Task UpdateReservation(Guid id, ReservationRequest request)
        {
            try
            {
                var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id);
                if (reservation == null) throw new Exception("Reserva não encontrada");

                reservation.UserId = request.UserId;
                reservation.LaboratoryId = request.LaboratoryId;
                reservation.ReservationDate = request.ReservationDate;
                reservation.StartTime = request.StartTime;
                reservation.EndTime = request.EndTime;
                reservation.UpdatedAt = DateTime.Now;

                await ValidateConflict(reservation);

                _context.Reservations.Update(reservation);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar reserva: " + ex.Message);
            }
        }

        public async Task<PaginatedResult<ReservationResponse>> SearchReservation(SearchReservationInput filter)
        {
            try
            {
                var query = _context.Reservations.AsQueryable();

                if (!string.IsNullOrWhiteSpace(filter.Keyword))
                {
                    query = query.Where(r => r.Laboratory.Name.Contains(filter.Keyword) ||
                                             r.User.Name.Contains(filter.Keyword));
                }

                if (filter.StartDate.HasValue)
                    query = query.Where(r => r.ReservationDate >= filter.StartDate.Value);

                if (filter.EndDate.HasValue)
                    query = query.Where(r => r.ReservationDate <= filter.EndDate.Value);

                query = filter.Order.ToUpper() == "ASC"
                    ? query.OrderBy(r => r.ReservationDate)
                    : query.OrderByDescending(r => r.ReservationDate);

                var responseQuery = query.Select(r => new ReservationResponse
                {
                    Id = r.Id,
                    ReservationDate = r.ReservationDate,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime,
                    UserId = r.UserId,
                    UserName = r.User.Name,
                    LaboratoryId = r.LaboratoryId,
                    LaboratoryName = r.Laboratory.Name
                });

                return await responseQuery.ToPaginatedResultAsync(filter.Page, filter.Size);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar reservas: " + ex.Message);
            }
        }

        private async Task ValidateConflict(Reservation reservation)
        {
            try
            {
                var hasConflict = await _context.Reservations.AnyAsync(r =>
                    r.Id != reservation.Id &&
                    r.LaboratoryId == reservation.LaboratoryId &&
                    r.ReservationDate.Date == reservation.ReservationDate.Date &&
                    ((reservation.StartTime >= r.StartTime && reservation.StartTime < r.EndTime) ||
                     (reservation.EndTime > r.StartTime && reservation.EndTime <= r.EndTime)));

                if (hasConflict)
                {
                    throw new Exception("O Laboratório já está reservado para este horário!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao validar conflito de reserva: " + ex.Message);
            }
        }

    }
}
