using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Readora.Services.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;

namespace Readora.Services;

public class FileSaver : IFileSaver
{
    public async Task<(string filePath, string hash)> SaveFile(IFormFile file, string envContentRootPath, string folder)
    {
        var uploadsPath = Path.Combine(envContentRootPath, "files", folder);
        Directory.CreateDirectory(uploadsPath);
    
        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        var isImage = file.ContentType.StartsWith("image/");

        var fileName = $"{Guid.NewGuid()}{(isImage ? ".webp" : fileExtension)}";
        var filePath = Path.Combine(uploadsPath, fileName);

        if (isImage)
        {
            using var image = await Image.LoadAsync(file.OpenReadStream());
            await image.SaveAsWebpAsync(filePath, new WebpEncoder
            {
                Quality = 75,
            });
        }
        else
        {
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
        }

        var hash = ComputeFileHash(filePath);
    
        return (Path.Combine(folder, fileName), hash);
    }

    private static string ComputeFileHash(string filePath)
    {
        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hashBytes = sha256.ComputeHash(stream);
        return Convert.ToHexString(hashBytes);
    }
}