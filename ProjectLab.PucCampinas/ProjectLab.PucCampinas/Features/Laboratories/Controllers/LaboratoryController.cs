using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectLab.PucCampinas.Common.Models;
using ProjectLab.PucCampinas.Features.Laboratories.DTOs;
using ProjectLab.PucCampinas.Features.Laboratories.Model;
using ProjectLab.PucCampinas.Features.Laboratories.Service;

namespace ProjectLab.PucCampinas.Features.Laboratories.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LaboratoryController : ControllerBase
    {
        private readonly ILaboratoryService _service;

        public LaboratoryController(ILaboratoryService service)
        {
            _service = service;
        }

        /// <summary>
        /// Realiza a busca paginada de laboratórios com filtros.
        /// </summary>
        /// <param name="filter">Filtros de busca e paginação.</param>
        /// <returns>Uma lista paginada de laboratórios.</returns>
        [HttpGet("search")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResult<LaboratoryResponse>>> SearchLaboratories([FromQuery] SearchLaboratoryInput filter)
        {
            return Ok(await _service.SearchLaboratories(filter));
        }

        /// <summary>
        /// Busca um laboratório específico pelo ID.
        /// </summary>
        /// <param name="id">Identificador único (Guid) do laboratório.</param>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LaboratoryResponse>> GetLaboratoriesById(Guid id)
        {
            var laboratory = await _service.GetLaboratoriesById(id);
            return Ok(laboratory);
        }

        /// <summary>
        /// Cadastra um novo laboratório no sistema.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<LaboratoryResponse>> CreateLaboratories(LaboratoryRequest request)
        {
            var response = await _service.CreateLaboratories(request);
            return CreatedAtAction(nameof(GetLaboratoriesById), new { id = response.Id }, response);
        }

        /// <summary>
        /// Atualiza os dados de um laboratório existente.
        /// </summary>
        /// <param name="laboratory">Objeto do laboratório com os dados atualizados.</param>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> UpdateLaboratories(Guid id, LaboratoryRequest request)
        {
            await _service.UpdateLaboratories(id, request);
            return NoContent();
        }

        /// <summary>
        /// Remove um laboratório do sistema permanentemente.
        /// </summary>
        /// <param name="id">ID do laboratório a ser removido.</param>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> DeleteLaboratories(Guid id)
        {
            await _service.DeleteLaboratories(id);
            return NoContent();
        }
    }
}