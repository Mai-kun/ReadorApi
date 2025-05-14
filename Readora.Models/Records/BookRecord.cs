using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Readora.Services;

public class BookRecord
{
    [Parameter("uint256", "bookId", 1)]
    public int BookId { get; set; }

    [Parameter("string", "title", 2)]
    public string Title { get; set; }

    [Parameter("address", "author", 3)]
    public string Author { get; set; }

    [Parameter("string", "fileHash", 4)]
    public string FileHash { get; set; }

    [Parameter("uint256", "timestamp", 5)]
    public long Timestamp { get; set; }
}