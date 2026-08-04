// Api/Controllers/UsuariosController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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

    [HttpGet]
    [Authorize(Roles = "admin")]  // Só admin lista
    public async Task<ActionResult<List<UsuarioResponse>>> ObterTodos()
    {
        return Ok(await _usuarioService.ObterTodos());
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioResponse>> Criar([FromBody] CriarUsuarioRequest request)
    {
        var usuario = await _usuarioService.Criar(request);
        return Ok(usuario);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "admin")]  // Só admin exclui
    public async Task<ActionResult> Excluir(Guid id)
    {
        var result = await _usuarioService.Excluir(id);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<UsuarioResponse>> Atualizar(Guid id, [FromBody] AtualizarUsuarioRequest request)
    {
        var usuario = await _usuarioService.Atualizar(id, request);
        if (usuario is null) return NotFound();
        return Ok(usuario);
    }
}