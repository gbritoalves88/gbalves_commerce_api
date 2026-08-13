// Api/Controllers/UsuariosController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GBAlves.Commerce.Application.DTOs.Usuarios;
using GBAlves.Commerce.Application.Interfaces;

namespace GBAlves.Commerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    /// <summary>
    //✅ ADMIN - Listar todos os usuários
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<List<UsuarioResponse>>> ObterTodos()
    {
        return Ok(await _usuarioService.ObterTodos());
    }

    /// <summary>
    /// ✅ PÚBLICO - Cadastro de novo usuário (cliente)
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<UsuarioResponse>> Criar([FromBody] CriarUsuarioRequest request)
    {
        try
        {
            var usuario = await _usuarioService.Criar(request);
            return Ok(usuario);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }
    

    /// <summary>
    //✅ ADMIN - Excluir usuário
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult> Excluir(Guid id)
    {
        var result = await _usuarioService.Excluir(id);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    //✅ CLIENTE - Atualizar próprio perfil (ou Admin atualizar qualquer)
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Cliente")]
    public async Task<ActionResult<UsuarioResponse>> Atualizar(Guid id, [FromBody] AtualizarUsuarioRequest request)
    {
        // Cliente só pode atualizar a si mesmo
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!User.IsInRole("Admin") && usuarioId != id.ToString())
            return Forbid();

        var usuario = await _usuarioService.Atualizar(id, request);
        if (usuario is null) return NotFound();
        return Ok(usuario);
    }
}