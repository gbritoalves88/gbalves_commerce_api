// Application/Interfaces/IUploadService.cs
using GBAlves.Commerce.Application.DTOs.Upload;

namespace GBAlves.Commerce.Application.Interfaces;

public interface IUploadService
{
    Task<UploadResponse> UploadBase64Async(UploadRequest request);
}