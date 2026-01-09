using Microsoft.AspNetCore.Authorization;
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
        [HttpPut("{id}")]
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

        /// <summary>
        /// Obtém o histórico de reservas do usuário logado.
        /// </summary>
        /// <remarks>
        /// Retorna uma lista contendo reservas futuras, passadas e canceladas.
        /// O ID do usuário é extraído automaticamente do Token JWT.
        /// </remarks>
        /// <returns>Uma lista de objetos ReservationResponse.</returns>
        /// <response code="200">Retorna a lista de reservas com sucesso.</response>
        /// <response code="401">Usuário não autenticado ou token inválido.</response>
        [HttpGet("my-reservations")]
        [ProducesResponseType(typeof(List<ReservationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ReservationResponse>>> GetMyReservation([FromQuery] Guid userId)
        {
            var reservations = await _service.GetMyReservation(userId);
            return Ok(reservations);
        }

        /// <summary>
        /// Cancela uma reserva existente.
        /// </summary>
        /// <remarks>
        /// Este método realiza uma exclusão lógica (Soft Delete), alterando o status para 'Cancelled'.
        /// Não é possível cancelar reservas de datas passadas.
        /// </remarks>
        /// <param name="id">O GUID da reserva a ser cancelada.</param>
        /// <response code="204">Reserva cancelada com sucesso (sem conteúdo de retorno).</response>
        /// <response code="400">Erro de validação (ex: tentar cancelar reserva passada).</response>
        /// <response code="404">Reserva não encontrada.</response>
        /// <response code="401">Não autorizado.</response>
        [HttpDelete("{id}/cancel")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CancelReservation(Guid id)
        {
                await _service.CancelReservation(id);
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
        /// <response code="200">Retorna a lista de horários ocupados.</response>
        [HttpGet("reservation-time")]
        [Authorize]
        [ProducesResponseType(typeof(List<ReservedTimes>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ReservedTimes>>> GetReservedTimes([FromQuery] Guid laboratoryId, [FromQuery] DateTime date)
        {
            var slots = await _service.GetReservedTimes(laboratoryId, date);
            return Ok(slots);
        }
    }
}
