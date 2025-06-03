using System.Numerics;
using Microsoft.AspNetCore.Mvc;
using Readora.Services;

namespace Readora.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlockchainController : ControllerBase
{
    private readonly BlockchainService _blockchainService;

    public BlockchainController(BlockchainService blockchainService)
    {
        _blockchainService = blockchainService;
    }

    [HttpGet("count")]
    public async Task<ActionResult<BigInteger>> GetBookCount()
    {
        var count = await _blockchainService.GetBookCount();
        return Ok(count.ToString());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetBook(BigInteger id)
    {
        var book = await _blockchainService.GetBook(id);
        return Ok(book);
    }
}