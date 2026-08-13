// Api/Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using GBAlves.Commerce.Application.DTOs.Auth;
using GBAlves.Commerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace GBAlves.Commerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Login público - qualquer pessoa pode acessar
    /// </summary>
    [HttpPost("login")]
    [EnableRateLimiting("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.Login(request);
            
            if (response is null)
            {
                _logger.LogWarning("Tentativa de login falhou para {Email}", request.Email);
                return Unauthorized(new { message = "Email ou senha inválidos" });
            }

            _logger.LogInformation("Login realizado: {Email} ({NivelAcesso})", 
                response.Email, response.NivelAcesso);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar login para {Email}", request.Email);
            return StatusCode(500, new { erro = "Erro interno ao processar login." });
        }
    }
}