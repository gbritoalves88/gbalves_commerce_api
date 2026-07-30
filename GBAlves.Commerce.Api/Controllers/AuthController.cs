// Api/Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using GBAlves.Commerce.Application.DTOs.Auth;
using GBAlves.Commerce.Application.Services;

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

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.Login(request);
        if (response is null)
            return Unauthorized(new { message = "Email ou senha inválidos" });

        return Ok(response);
    }
}