using Microsoft.AspNetCore.Mvc;
using ProjectLab.PucCampinas.Common.Models;
using ProjectLab.PucCampinas.Features.Auth.Service;
using ProjectLab.PucCampinas.Features.Users.DTOs;
using ProjectLab.PucCampinas.Features.Users.Model;
using ProjectLab.PucCampinas.Features.Users.Service;
using ProjectLab.PucCampinas.Features.Users.Service.shared;

namespace ProjectLab.PucCampinas.Features.Users.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IViaCepService _viaCepService;

        public UserController(IUserService service, IViaCepService viaCepService)
        {
            _service = service;
            _viaCepService = viaCepService;
        }

        /// <summary>
        /// Realiza a busca paginada de usuários com seus definitivos cargos.
        /// </summary>
        /// <param name="filter">Filtros por nome, email ou telefone.</param>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResult<UserResponse>>> SearchUser([FromQuery] SearchUserInput filter)
        {
            return Ok(await _service.SearchUser(filter));
        }

        /// <summary>
        /// Obtém os detalhes de um usuário e suas reservas vinculadas.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResponse>> GetUserById(Guid id)
        {
            var response = await _service.GetUserById(id);
            if (response == null) return NotFound("Usuário não encontrado.");

            return Ok(response);
        }

        /// <summary>
        /// Cadastra um novo usuário no sistema.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<UserResponse>> CreateUser(UserRequest request, AuthService authService)
        {
            var response = await _service.CreateUser(request, authService);
            return CreatedAtAction(nameof(GetUserById), new { id = response.Id }, response);
        }

        /// <summary>
        /// Atualiza os dados cadastrais do usuário.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateUser(Guid id, UserRequest request)
        {
            await _service.UpdateUser(id, request);
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

        [HttpGet("consult-cep/{cep}")]
        public async Task<IActionResult> GetAddressByCep(string cep)
        {
            var cepLimpo = cep.Replace("-", "").Trim();

            if (cepLimpo.Length != 8)
                return BadRequest("CEP inválido.");

            var address = await _viaCepService.GetAddressByCep(cepLimpo);

            if (address == null)
                return NotFound("CEP não encontrado.");

            return Ok(address);
        }
    }
}
