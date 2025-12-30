using ProjectLab.PucCampinas.Common.Models;
using ProjectLab.PucCampinas.Features.Reservations.DTOs;
using ProjectLab.PucCampinas.Features.Reservations.Model;

namespace ProjectLab.PucCampinas.Features.Reservations.Service
{
    public interface IReservationService
    {
        Task<PaginatedResult<Reservation>> SearchReservation(SearchReservationInput filters);
        Task<Reservation?> GetReservationById(Guid id);
        Task CreateReservation(Reservation reservation);
        Task UpdateReservation(Reservation reservation);
        Task DeleteReservation(Guid id);
    }
}
