using Microsoft.AspNetCore.Mvc;
using StoreTransactionManager.Core.Interfaces;

namespace StoreTransactionManager.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class GoodController : ControllerBase
{
    private readonly IGoodsService _goodService;

    public GoodController(IGoodsService goodService)
    {
        _goodService = goodService;
    }

    [HttpGet("GetGoodTransactions")]
    public async Task<IActionResult> GetGoodTransactions([FromQuery] int goodID, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
    {
        if (IsNotValidRequestParameters(goodID, fromDate, toDate))
            return BadRequest();

        var result = await _goodService.GetGoodTransactionsAsync(goodID, fromDate, toDate);


        if (result == null)
            return NotFound();


        return Ok(result);
    }

    private static bool IsNotValidRequestParameters(int goodID, DateTime fromDate, DateTime toDate)
    {
        return goodID <= 0 || fromDate > toDate || fromDate == DateTime.MinValue || toDate == DateTime.MinValue;
    }
}
