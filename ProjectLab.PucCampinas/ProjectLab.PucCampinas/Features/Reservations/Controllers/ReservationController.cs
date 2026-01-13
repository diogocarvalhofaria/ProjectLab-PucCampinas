using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectLab.PucCampinas.Common.Models;
using ProjectLab.PucCampinas.Features.Reservations.DTOs;
using ProjectLab.PucCampinas.Features.Reservations.Model;
using ProjectLab.PucCampinas.Features.Reservations.Service;
using System.Security.Claims;

namespace ProjectLab.PucCampinas.Features.Reservations.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _service;

        public ReservationController(IReservationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Busca reservas com filtros de data, palavras-chave e paginação.
        /// </summary>
        /// <param name="filter">Filtros de busca (Keyword, StartDate, EndDate).</param>
        [HttpGet("search")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResult<ReservationResponse>>> SearchReservation([FromQuery] SearchReservationInput filter)
        {
            return Ok(await _service.SearchReservation(filter));
        }

        /// <summary>
        /// Obtém os detalhes de uma reserva específica por ID.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReservationResponse>> GetReservationById(Guid id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

            var currentUserId = Guid.Parse(userIdString);
            var isAdmin = User.IsInRole("Admin");

            var reservation = await _service.GetReservationById(id, currentUserId, isAdmin);
            return Ok(reservation);
   
        }

        /// <summary>
        /// Cria uma nova reserva de laboratório.
        /// </summary>
        /// <remarks>
        /// Valida se o laboratório já possui agendamento para o período solicitado.
        /// </remarks>
        /// <response code="201">Reserva criada com sucesso.</response>
        /// <response code="400">Erro de conflito de horário ou dados inválidos.</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ReservationResponse>> CreateReservation(ReservationRequest request)
        {
            var response = await _service.CreateReservation(request);
            return CreatedAtAction(nameof(GetReservationById), new { id = response.Id }, response);
        }

        /// <summary>
        /// Atualiza uma reserva existente.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdateReservation(Guid id, ReservationRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserId = Guid.Parse(userIdString);
            var isAdmin = User.IsInRole("Admin");

            await _service.UpdateReservation(id, request, currentUserId, isAdmin);

            return NoContent();
        }

        /// <summary>
        /// Cancela/Exclui uma reserva permanentemente.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> DeleteReservation(Guid id)
        {
            await _service.DeleteReservation(id);
            return NoContent();
        }

        /// <summary>
        /// Obtém o histórico de reservas do usuário logado.
        /// </summary>
        /// <remarks>
        /// Retorna uma lista contendo reservas futuras, passadas e canceladas.
        /// </remarks>
        /// <returns>Uma lista de objetos ReservationResponse.</returns>
        [HttpGet("my-reservations")]
        [Authorize]
        [ProducesResponseType(typeof(List<ReservationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ReservationResponse>>> GetMyReservation()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

            var currentUserId = Guid.Parse(userIdString);

            var reservations = await _service.GetMyReservation(currentUserId);
            return Ok(reservations);
        }

        /// <summary>
        /// Cancela uma reserva existente (Soft Delete / Status Cancelled).
        /// </summary>
        /// <remarks>
        /// Este método realiza uma exclusão lógica, alterando o status para 'Cancelled'.
        /// </remarks>
        /// <param name="id">O GUID da reserva a ser cancelada.</param>
        [HttpDelete("{id}/cancel")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> CancelReservation(Guid id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("Token inválido: ID do usuário não encontrado.");
            }

            var currentUserId = Guid.Parse(userIdString);
            var isAdmin = User.IsInRole("Admin");

            await _service.CancelReservation(id, currentUserId, isAdmin);
            return NoContent();
        }

        /// <summary>
        /// Consulta os horários já ocupados de um laboratório em uma data específica.
        /// </summary>
        /// <remarks>
        /// Útil para montar a grade de horários no Front-end e bloquear slots indisponíveis.
        /// </remarks>
        /// <param name="laboratoryId">O ID do laboratório.</param>
        /// <param name="date">A data para consulta (ex: 2024-12-31).</param>
        /// <returns>Lista de horários (Início e Fim) que já possuem reserva confirmada.</returns>
        [HttpGet("reservation-time")]
        [Authorize]
        [ProducesResponseType(typeof(List<ReservedTimes>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ReservedTimes>>> GetReservedTimes([FromQuery] Guid laboratoryId, [FromQuery] DateTime date)
        {
            var slots = await _service.GetReservedTimes(laboratoryId, date);
            return Ok(slots);
        }
    }
}