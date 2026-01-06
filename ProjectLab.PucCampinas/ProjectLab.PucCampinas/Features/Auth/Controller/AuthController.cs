using Microsoft.AspNetCore.Mvc;
using ProjectLab.PucCampinas.Features.Auth.DTOs;
using ProjectLab.PucCampinas.Features.Auth.Service;
using ProjectLab.PucCampinas.Features.Users.DTOs;

namespace ProjectLab.PucCampinas.Features.Auth.Controller
{
        [ApiController]
        [Route("api/[controller]")]
        public class AuthController : ControllerBase
        {
            private readonly AuthService _authService;

            public AuthController(AuthService authService)
            {
                _authService = authService;
            }

        /// <summary>
        /// Define a senha de acesso para um usuário (Primeiro Acesso).
        /// </summary>
        /// <remarks>
        /// Utilize este endpoint para simular o fluxo onde o usuário clica no link do e-mail e define sua senha pela primeira vez.
        /// O sistema buscará o usuário pelo RA e atualizará o hash da senha.
        /// </remarks>
        /// <param name="request">Objeto contendo o RA e a nova Senha.</param>
        /// <response code="200">Senha definida com sucesso.</response>
        /// <response code="400">Usuário não encontrado ou erro na solicitação.</response>
        [HttpPost("setup-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetupPassword([FromBody] SetupPasswordRequest request)
        {
            var ra = _authService.ValidateTokenAndGetRa(request.Token);
            await _authService.SetPassword(ra, request.NewPassword);

            return Ok(new { message = "Senha definida com sucesso!" });
        }
       

        /// <summary>
        /// Realiza o login no sistema.
        /// </summary>
        /// <remarks>
        /// Valida o RA e a Senha. Se corretos, retorna um Token JWT.
        /// Este Token deve ser enviado no cabeçalho "Authorization" das requisições protegidas (Bearer Token).
        /// </remarks>
        /// <param name="request">Credenciais de acesso (RA e Senha).</param>
        /// <returns>Objeto contendo dados do usuário e o Token JWT.</returns>
        /// <response code="200">Login realizado com sucesso.</response>
        /// <response code="401">Credenciais inválidas (RA ou senha incorretos).</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
            {
                    var response = await _authService.Login(request);
                    return Ok(response);
                
            }
        }
}
