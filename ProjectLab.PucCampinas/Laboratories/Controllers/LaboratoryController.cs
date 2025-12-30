using Microsoft.AspNetCore.Mvc;
using ProjectLab.PucCampinas.Common.Models;
using ProjectLab.PucCampinas.Laboratories.DTOs;
using ProjectLab.PucCampinas.Laboratories.Model;
using ProjectLab.PucCampinas.Laboratories.Service;

namespace ProjectLab.PucCampinas.Laboratories.Controllers
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResult<Laboratory>>> SearchLaboratories([FromQuery] SearchLaboratoryInput filter)
        {
            var result = await _service.SearchLaboratories(filter);
            return Ok(result);
        }

        /// <summary>
        /// Busca um laboratório específico pelo ID.
        /// </summary>
        /// <param name="id">Identificador único (Guid) do laboratório.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Laboratory>> GetLaboratoriesById(Guid id)
        {
            var laboratory = await _service.GetLaboratoriesById(id);
            if (laboratory == null) return NotFound("Laboratório não encontrado.");

            return Ok(laboratory);
        }

        /// <summary>
        /// Cadastra um novo laboratório no sistema.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> CreateLaboratories(Laboratory laboratory)
        {
            await _service.CreateLaboratories(laboratory);
            return CreatedAtAction(nameof(GetLaboratoriesById), new { id = laboratory.Id }, laboratory);
        }

        /// <summary>
        /// Atualiza os dados de um laboratório existente.
        /// </summary>
        /// <param name="laboratory">Objeto do laboratório com os dados atualizados.</param>
        /// <response code="204">Laboratório atualizado com sucesso.</response>
        /// <response code="400">Dados inválidos enviados na requisição.</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateLaboratories(Laboratory laboratory)
        {
            await _service.UpdateLaboratories(laboratory);
            return NoContent();
        }

        /// <summary>
        /// Remove um laboratório do sistema permanentemente.
        /// </summary>
        /// <param name="id">ID do laboratório a ser removido.</param>
        /// <response code="204">Laboratório removido com sucesso.</response>
        /// <response code="404">Laboratório não encontrado para exclusão.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteLaboratories(Guid id)
        {
            await _service.DeleteLaboratories(id);
            return NoContent();
        }
    }
}
