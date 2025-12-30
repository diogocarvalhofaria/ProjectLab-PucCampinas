using Microsoft.AspNetCore.Mvc;
using ProjectLab.PucCampinas.Common.Models;
using ProjectLab.PucCampinas.Users.DTOs;
using ProjectLab.PucCampinas.Users.Model;
using ProjectLab.PucCampinas.Users.Service;

namespace ProjectLab.PucCampinas.Users.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        /// <summary>
        /// Realiza a busca paginada de usuários com seus definitivos cargos.
        /// </summary>
        /// <param name="filter">Filtros por nome, email ou telefone.</param>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResult<User>>> SearchUser([FromQuery] SearchUserInput filter)
        {
            return Ok(await _service.SearchUser(filter));
        }

        /// <summary>
        /// Obtém os detalhes de um usuário e suas reservas vinculadas.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetUserById(Guid id)
        {
            var user = await _service.GetUserById(id);
            if (user == null) return NotFound("Usuário não encontrado.");
            return Ok(user);
        }

        /// <summary>
        /// Cadastra um novo usuário no sistema.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> CreateUser(User user)
        {
            await _service.CreateUser(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        /// <summary>
        /// Atualiza os dados cadastrais do usuário.
        /// </summary>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateUser(User user)
        {
            await _service.UpdateUser(user);
            return NoContent();
        }

        /// <summary>
        /// Remove um usuário do sistema.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            await _service.DeleteUser(id);
            return NoContent();
        }
    }
}
