using Microsoft.AspNetCore.Http;

namespace Readora.Services.Abstractions;

public interface IFileSaver
{
    Task<(string filePath, string hash)> SaveFile(IFormFile file, string envContentRootPath, string folder);
}