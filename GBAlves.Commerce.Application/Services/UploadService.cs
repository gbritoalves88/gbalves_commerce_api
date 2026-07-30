// Application/Services/UploadService.cs
using System.Text.RegularExpressions;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using GBAlves.Commerce.Application.DTOs.Upload;
using GBAlves.Commerce.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GBAlves.Commerce.Application.Services;

public class UploadService : IUploadService
{
    private readonly Cloudinary _cloudinary;

    public UploadService(IConfiguration configuration)
    {
        var cloudinaryUrl = configuration["Cloudinary:Url"];
        _cloudinary = new Cloudinary(cloudinaryUrl);
        _cloudinary.Api.Secure = true;
    }

    public async Task<UploadResponse> UploadBase64Async(UploadRequest request)
    {
        var base64Limpo = Regex.Replace(request.Base64, @"^data:image\/[a-zA-Z]+;base64,", "");

        var bytes = Convert.FromBase64String(base64Limpo);
        using var stream = new MemoryStream(bytes);

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription($"produto_{Guid.NewGuid()}", stream),
            Folder = "gbalves-commerce"
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
            throw new Exception(result.Error.Message);

        return new UploadResponse(result.SecureUrl.ToString());
    }
}