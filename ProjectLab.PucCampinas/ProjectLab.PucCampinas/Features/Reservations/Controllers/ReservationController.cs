using Microsoft.AspNetCore.Mvc;
using ProjectLab.PucCampinas.Common.Models;
using ProjectLab.PucCampinas.Features.Reservations.DTOs;
using ProjectLab.PucCampinas.Features.Reservations.Model;
using ProjectLab.PucCampinas.Features.Reservations.Service;

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResult<ReservationResponse>>> SearchReservation([FromQuery] SearchReservationInput filter)
        {
            return Ok(await _service.SearchReservation(filter));
        }

        /// <summary>
        /// Obtém os detalhes de uma reserva específica por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReservationResponse>> GetReservationById(Guid id)
        {
            var reservation = await _service.GetReservationById(id);
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReservationResponse>> CreateReservation(ReservationRequest request)
        {
            var response = await _service.CreateReservation(request);
            return CreatedAtAction(nameof(GetReservationById), new { id = response.Id }, response);
        }

        /// <summary>
        /// Atualiza uma reserva existente.
        /// </summary>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateReservation(Guid id, ReservationRequest request)
        {
            await _service.UpdateReservation(id, request);
            return NoContent();
        }

        /// <summary>
        /// Cancela/Exclui uma reserva.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteReservation(Guid id)
        {
            await _service.DeleteReservation(id);
            return NoContent();
        }
    }
}
