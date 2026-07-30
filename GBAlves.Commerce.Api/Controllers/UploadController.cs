// Api/Controllers/UploadController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GBAlves.Commerce.Application.DTOs.Upload;
using GBAlves.Commerce.Application.Interfaces;

namespace GBAlves.Commerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly IUploadService _uploadService;

    public UploadController(IUploadService uploadService)
    {
        _uploadService = uploadService;
    }

    [HttpPost("imagem")]
    public async Task<ActionResult<UploadResponse>> UploadImagem([FromBody] UploadRequest request)
    {
        try
        {
            var response = await _uploadService.UploadBase64Async(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpGet("teste")]
    public ActionResult Teste()
    {
        var usuario = User.Identity?.Name;
        return Ok(new { autenticado = User.Identity?.IsAuthenticated, usuario });
    }
}