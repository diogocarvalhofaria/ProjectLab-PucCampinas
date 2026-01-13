using Microsoft.EntityFrameworkCore;
using ProjectLab.PucCampinas.Common.Models;
using ProjectLab.PucCampinas.Common.Services;
using ProjectLab.PucCampinas.Features.Reservations.DTOs;
using ProjectLab.PucCampinas.Features.Reservations.Model;
using ProjectLab.PucCampinas.Infrastructure.Data;
using ProjectLab.PucCampinas.shared.Service;
using System.Numerics;

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

        public async Task<ReservationResponse?> GetReservationById(Guid id, Guid currentUserId, bool isAdmin)
        {
            try
            {
                var r = await _context.Reservations
                    .Include(x => x.Laboratory)
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (r == null) return null;

                if (!isAdmin && r.UserId != currentUserId)
                {
                    throw new UnauthorizedAccessException("Acesso negado aos detalhes desta reserva.");
                }

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

        public async Task UpdateReservation(Guid id, ReservationRequest request, Guid currentUserId, bool isAdmin)
        {
            try
            {
                var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id);
                if (reservation == null) throw new Exception("Reserva não encontrada");

                if (!isAdmin && reservation.UserId != currentUserId)
                {
                    throw new UnauthorizedAccessException("Você não pode editar uma reserva que não é sua.");
                }

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
                    LaboratoryName = r.Laboratory.Name,
                    Status = r.Status.ToString()
                });

                return await responseQuery.ToPaginatedResultAsync(filter.Page, filter.Size);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar reservas: " + ex.Message);
            }
        }

        public async Task<List<Reservation>> GetByUserId(Guid userId)
        {
            try
            {
                return await _context.Reservations
                    .Include(r => r.Laboratory)
                    .Where(r => r.UserId == userId)
                    .OrderByDescending(r => r.ReservationDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter reserva por usuário: " + ex.Message);
            }
        }

        public async Task<List<ReservationResponse>> GetMyReservation(Guid userId)
        {
            try
            {
                var reservations = await GetByUserId(userId);

                return reservations.Select(r => new ReservationResponse
                {
                    Id = r.Id,
                    LaboratoryName = r.Laboratory?.Name ?? "Desconhecido",
                    ReservationDate = r.ReservationDate,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime,
                    UserId = r.UserId,
                    UserName = r.User?.Name ?? "",
                    LaboratoryId = r.LaboratoryId,
                    Status = r.Status.ToString()
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter reservas do usuário: " + ex.Message);
            }
        }

        public async Task CancelReservation(Guid id, Guid currentUserId, bool isAdmin)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);

                if (reservation == null)
                    throw new Exception("Reserva não encontrada.");

                if (reservation.ReservationDate < DateTime.Today)
                    throw new Exception("Não é possível cancelar reservas passadas.");

                if (!isAdmin && reservation.UserId != currentUserId)
                {
                    throw new UnauthorizedAccessException("Você não pode cancelar a reserva de outra pessoa.");
                }

                reservation.Status = ReservationStatus.Cancelled;
                reservation.UpdatedAt = DateTime.Now;

                _context.Reservations.Update(reservation);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao cancelar reserva: " + ex.Message);
            }
        }

        public async Task<List<ReservedTimes>> GetReservedTimes(Guid laboratoryId, DateTime date)
        {
            try
            {
                var reservations = await _context.Reservations
                    .Where(r => r.LaboratoryId == laboratoryId &&
                                r.ReservationDate.Date == date.Date &&
                                r.Status != ReservationStatus.Cancelled)
                    .Select(r => new ReservedTimes
                    {
                        StartTime = r.StartTime.ToString("HH:mm"),
                        EndTime = r.EndTime.ToString("HH:mm")
                    })
                    .ToListAsync();

                return reservations;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar horários ocupados: " + ex.Message);
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
