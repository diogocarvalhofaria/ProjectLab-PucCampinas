using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginatedResult<UserResponse>>> SearchUser([FromQuery] SearchUserInput filter)
        {
            return Ok(await _service.SearchUser(filter));
        }

        /// <summary>
        /// Obtém os detalhes de um usuário e suas reservas vinculadas.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UserResponse>> CreateUser(UserRequest request, [FromServices] AuthService authService)
        {
            var response = await _service.CreateUser(request, authService);
            return CreatedAtAction(nameof(GetUserById), new { id = response.Id }, response);
        }

        /// <summary>
        /// Atualiza os dados cadastrais do usuário.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> UpdateUser(Guid id, UserRequest request)
        {
            await _service.UpdateUser(id, request);
            return NoContent();
        }

        /// <summary>
        /// Remove um usuário do sistema.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            await _service.DeleteUser(id);
            return NoContent();
        }

        [HttpGet("consult-cep/{cep}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Reenvia o e-mail de definição de senha para um usuário.
        /// </summary>
        /// <remarks>
        /// Gera um novo token de configuração (válido por 24h) e envia o template de e-mail.
        /// <br/>
        /// <b>Regra de Negócio:</b> Este endpoint só processa a solicitação se o usuário estiver 
        /// marcado como <b>Inativo</b> (ou seja, ainda não definiu sua senha inicial).
        /// Se o usuário já estiver ativo, retornará um erro.
        /// </remarks>
        /// <param name="id">O identificador único (GUID) do usuário.</param>
        /// <returns>Retorna status 204 (No Content) se o envio for iniciado com sucesso.</returns>
        /// <response code="204">Sucesso. O e-mail foi reenviado.</response>
        /// <response code="400">Erro de validação (ex: Usuário já está ativo/possui senha).</response>
        /// <response code="404">Usuário não encontrado com o ID informado.</response>
        /// <response code="500">Erro interno ao tentar enviar o e-mail.</response>
        [HttpPost("{id}/resend-email")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResendEmail(Guid id)
        {
            await _service.ResendEmail(id);
            return NoContent();
        }
    }
}