using ProjectLab.PucCampinas.Common.Models;
using ProjectLab.PucCampinas.Features.Reservations.DTOs;
using ProjectLab.PucCampinas.Features.Reservations.Model;

namespace ProjectLab.PucCampinas.Features.Reservations.Service
{
    public interface IReservationService
    {
        Task<PaginatedResult<ReservationResponse>> SearchReservation(SearchReservationInput filters);
        Task<ReservationResponse?> GetReservationById(Guid id, Guid currentUserId, bool isAdmin);
        Task<ReservationResponse> CreateReservation(ReservationRequest request);
        Task UpdateReservation(Guid id, ReservationRequest request, Guid currentUserId, bool isAdmin);
        Task DeleteReservation(Guid id);
        Task<List<Reservation>> GetByUserId(Guid userId);
        Task CancelReservation(Guid id, Guid currentUserId, bool isAdmin);
        Task<List<ReservedTimes>> GetReservedTimes(Guid laboratoryId, DateTime date);
        Task<List<ReservationResponse>> GetMyReservation(Guid userId);
    }
}
