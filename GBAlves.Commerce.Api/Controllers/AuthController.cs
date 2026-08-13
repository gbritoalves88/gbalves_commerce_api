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

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Login público - qualquer pessoa pode acessar
    /// </summary>
    [HttpPost("login")]
    [EnableRateLimiting("login")]  // ← Adicione isso
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.Login(request);
        if (response is null)
            return Unauthorized(new { message = "Email ou senha inválidos" });

        return Ok(response);
    }

    
}