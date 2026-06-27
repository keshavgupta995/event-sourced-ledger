using LedgerApi.DTOs;
using LedgerApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LedgerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly AccountService _accountService;

    public TransactionsController(AccountService accountService)
    {
        _accountService = accountService;
    }

    // POST /api/transactions/deposit
    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] DepositMoneyDto dto)
    {
        try
        {
            var result = await _accountService.DepositAsync(dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // POST /api/transactions/withdraw
    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawMoneyDto dto)
    {
        try
        {
            var result = await _accountService.WithdrawAsync(dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // POST /api/transactions/transfer
    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferMoneyDto dto)
    {
        try
        {
            var result = await _accountService.TransferAsync(dto);
            return Ok(new { message = result });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}