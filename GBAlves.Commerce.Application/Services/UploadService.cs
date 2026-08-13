// Application/Services/UploadService.cs
using System.Text.RegularExpressions;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using GBAlves.Commerce.Application.DTOs.Upload;
using GBAlves.Commerce.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GBAlves.Commerce.Application.Services;

public class UploadService : IUploadService
{
    private readonly Cloudinary _cloudinary;
    private readonly ILogger<UploadService> _logger;

    // Limites
    private const int MAX_FILE_SIZE_MB = 5;
    private const int MAX_FILE_SIZE_BYTES = MAX_FILE_SIZE_MB * 1024 * 1024;
    private static readonly string[] ALLOWED_EXTENSIONS = { ".jpg", ".jpeg", ".png", ".webp" };

    public UploadService(IConfiguration configuration, ILogger<UploadService> logger)
    {
        var cloudinaryUrl = configuration["Cloudinary:Url"];
        _cloudinary = new Cloudinary(cloudinaryUrl);
        _cloudinary.Api.Secure = true;
        _logger = logger;
    }

    public async Task<UploadResponse> UploadBase64Async(UploadRequest request)
    {
        // ========== 1. VALIDAR SE O BASE64 ESTÁ VAZIO ==========
        if (string.IsNullOrWhiteSpace(request.Base64))
            throw new ArgumentException("Base64 não pode ser vazio!");

        // ========== 2. LIMPAR O BASE64 ==========
        var base64Limpo = Regex.Replace(request.Base64, @"^data:image\/[a-zA-Z]+;base64,", "");

        // ========== 3. VALIDAR TAMANHO DO BASE64 (ANTES DE CONVERTER) ==========
        // Estimativa: base64 tem ~33% overhead. Verifica se é muito grande
        if (base64Limpo.Length > MAX_FILE_SIZE_BYTES * 1.5)
            throw new ArgumentException($"Imagem muito grande! Máximo permitido: {MAX_FILE_SIZE_MB}MB");

        // ========== 4. CONVERTER BASE64 PARA BYTES ==========
        byte[] bytes;
        try
        {
            bytes = Convert.FromBase64String(base64Limpo);
        }
        catch (FormatException)
        {
            _logger.LogWarning("Tentativa de upload com base64 inválido");
            throw new ArgumentException("Base64 inválido!");
        }

        // ========== 5. VALIDAR TAMANHO REAL DO ARQUIVO ==========
        if (bytes.Length > MAX_FILE_SIZE_BYTES)
            throw new ArgumentException($"Imagem muito grande! Máximo permitido: {MAX_FILE_SIZE_MB}MB");

        if (bytes.Length == 0)
            throw new ArgumentException("Arquivo vazio!");

        // ========== 6. VALIDAR TIPO REAL DO ARQUIVO (MAGIC NUMBERS) ==========
        var contentType = GetContentType(bytes);
        if (contentType == "unknown")
        {
            _logger.LogWarning("Tentativa de upload com formato não suportado");
            throw new ArgumentException("Formato de imagem não suportado!");
        }

        // ========== 7. VALIDAR EXTENSÃO ==========
        var extension = contentType switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => throw new ArgumentException("Formato de imagem não suportado!")
        };

        if (!ALLOWED_EXTENSIONS.Contains(extension))
            throw new ArgumentException("Formato de imagem não suportado!");

        // ========== 8. FAZER UPLOAD PARA CLOUDINARY ==========
        using var stream = new MemoryStream(bytes);
        
        var fileName = $"produto_{Guid.NewGuid()}{extension}";

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, stream),
            Folder = "gbalves-commerce",
            PublicId = $"produtos/{Guid.NewGuid()}",
            Transformation = new Transformation()
                .Quality("auto")
                .FetchFormat("auto")
        };

        try
        {
            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                throw new Exception(result.Error.Message);

            _logger.LogInformation("✅ Upload realizado: {Url}", result.SecureUrl);
            return new UploadResponse(result.SecureUrl.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer upload para Cloudinary");
            throw new Exception("Erro ao fazer upload da imagem");
        }
    }

    // ========== MÉTODO AUXILIAR: Detectar tipo real do arquivo ==========
    private string GetContentType(byte[] bytes)
    {
        // JPEG: FF D8 FF
        if (bytes.Length > 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
            return "image/jpeg";

        // PNG: 89 50 4E 47 0D 0A 1A 0A
        if (bytes.Length > 7 && 
            bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47 &&
            bytes[4] == 0x0D && bytes[5] == 0x0A && bytes[6] == 0x1A && bytes[7] == 0x0A)
            return "image/png";

        // WebP: "RIFF" + "WEBP"
        if (bytes.Length > 11 && 
            bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46 &&
            bytes[8] == 0x57 && bytes[9] == 0x45 && bytes[10] == 0x42 && bytes[11] == 0x50)
            return "image/webp";

        // GIF: "GIF8"
        if (bytes.Length > 3 && 
            bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x38)
            return "image/gif";

        return "unknown";
    }
}