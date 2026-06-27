using LedgerApi.DTOs;
using LedgerApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LedgerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly AccountService _accountService;

    public AccountsController(AccountService accountService)
    {
        _accountService = accountService;
    }

    // POST /api/accounts
    [HttpPost]
    public async Task<IActionResult> OpenAccount([FromBody] OpenAccountDto dto)
    {
        try
        {
            var result = await _accountService.OpenAccountAsync(dto);
            return CreatedAtAction(nameof(GetAccount), new { accountId = result.AccountId }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // GET /api/accounts
    [HttpGet]
    public async Task<IActionResult> GetAllAccounts()
    {
        var result = await _accountService.GetAllAccountsAsync();
        return Ok(result);
    }

    // GET /api/accounts/{accountId}
    [HttpGet("{accountId}")]
    public async Task<IActionResult> GetAccount(string accountId)
    {
        try
        {
            var result = await _accountService.GetAccountAsync(accountId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    // GET /api/accounts/{accountId}/history
    [HttpGet("{accountId}/history")]
    public async Task<IActionResult> GetEventHistory(string accountId)
    {
        try
        {
            var result = await _accountService.GetEventHistoryAsync(accountId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}