using System.Security.Cryptography;
using System.Text;
using Readora.Models;

namespace Readora.Services.Helpers;

public static class BlockchainHelper
{
    public static string GenerateTransactionHash(Book book)
    {
        var rawData = $"{book.Id}|{book.Title}|{book.Author.Id}|" +
                      $"{book.UploadDate:yyyyMMddHHmmss}|{book.FileHash}";

        var bytes = Encoding.UTF8.GetBytes(rawData);
        var hashBytes = SHA256.HashData(bytes);

        var sb = new StringBuilder();
        foreach (var b in hashBytes)
        {
            sb.Append(b.ToString("x2"));
        }

        return $"LIB_{sb}";
    }
}
