using ProjectLab.PucCampinas.Common.Models;
using ProjectLab.PucCampinas.Reservations.DTOs;
using ProjectLab.PucCampinas.Reservations.Model;

namespace ProjectLab.PucCampinas.Reservations.Service
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
