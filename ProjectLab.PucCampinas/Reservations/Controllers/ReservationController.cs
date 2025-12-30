using Microsoft.AspNetCore.Mvc;
using ProjectLab.PucCampinas.Common.Models;
using ProjectLab.PucCampinas.Reservations.DTOs;
using ProjectLab.PucCampinas.Reservations.Model;
using ProjectLab.PucCampinas.Reservations.Service;

namespace ProjectLab.PucCampinas.Reservations.Controllers
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
        public async Task<ActionResult<PaginatedResult<Reservation>>> SearchReservation([FromQuery] SearchReservationInput filter)
        {
            return Ok(await _service.SearchReservation(filter));
        }

        /// <summary>
        /// Obtém os detalhes de uma reserva específica por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Reservation>> GetReservationById(Guid id)
        {
            var reservation = await _service.GetReservationById(id);
            if (reservation == null) return NotFound("Reserva não encontrada.");
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
        public async Task<ActionResult> CreateReservation(Reservation reservation)
        {
            try
            {
                await _service.CreateReservation(reservation);
                return CreatedAtAction(nameof(GetReservationById), new { id = reservation.Id }, reservation);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza uma reserva existente.
        /// </summary>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateReservation(Reservation reservation)
        {
            await _service.UpdateReservation(reservation);
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
