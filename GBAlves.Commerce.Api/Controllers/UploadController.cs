// Api/Controllers/UploadController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GBAlves.Commerce.Application.DTOs.Upload;
using GBAlves.Commerce.Application.Interfaces;

namespace GBAlves.Commerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Admin")]
public class UploadController : ControllerBase
{
    private readonly IUploadService _uploadService;
    private readonly ILogger<UploadController> _logger;
    
    // Constante para o limite de tamanho
    private const int MAX_FILE_SIZE_MB = 5;

    public UploadController(IUploadService uploadService, ILogger<UploadController> logger)
    {
        _uploadService = uploadService;
        _logger = logger;
    }

    /// <summary>
    /// ✅ ADMIN - Upload de imagem
    /// </summary>
    [HttpPost("imagem")]
    [RequestSizeLimit(MAX_FILE_SIZE_MB * 1024 * 1024)]
    public async Task<ActionResult<UploadResponse>> UploadImagem([FromBody] UploadRequest request)
    {
        try
        {
            var response = await _uploadService.UploadBase64Async(request);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Upload inválido: {Erro}", ex.Message);
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no upload");
            return StatusCode(500, new { erro = "Erro interno no servidor" });
        }
    }
}