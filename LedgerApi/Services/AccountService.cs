using LedgerApi.Data;
using LedgerApi.Data.Models;
using LedgerApi.Domain.Events;
using LedgerApi.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LedgerApi.Services;

public class AccountService
{
    private readonly LedgerDbContext _db;

    public AccountService(LedgerDbContext db)
    {
        _db = db;
    }

    // ─── Open Account ────────────────────────────────────────────────
    public async Task<AccountResponseDto> OpenAccountAsync(OpenAccountDto dto)
    {
        // check if owner already has an account
        var existingAccount = await _db.AccountBalances
            .FirstOrDefaultAsync(a => a.OwnerName.ToLower() == dto.OwnerName.ToLower());

        if (existingAccount != null)
            throw new InvalidOperationException($"An account for '{dto.OwnerName}' already exists.");

        var accountId = Guid.NewGuid().ToString();

        // build the event
        var @event = new AccountOpenedEvent
        {
            AccountId = accountId,
            OwnerName = dto.OwnerName,
            InitialDeposit = dto.InitialDeposit
        };

        // get next version number for this account
        var version = await GetNextVersionAsync(accountId);

        // append event to ledger_events
        await AppendEventAsync(accountId, "AccountOpened", dto.InitialDeposit, @event, version);

        // create projection in account_balances
        var balance = new AccountBalance
        {
            AccountId = accountId,
            OwnerName = dto.OwnerName,
            Balance = dto.InitialDeposit,
            CreatedAt = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        _db.AccountBalances.Add(balance);
        await _db.SaveChangesAsync();

        return MapToResponseDto(balance);
    }

    // ─── Deposit ─────────────────────────────────────────────────────
    public async Task<AccountResponseDto> DepositAsync(DepositMoneyDto dto)
    {
        var account = await GetAccountOrThrowAsync(dto.AccountId);

        var @event = new MoneyDepositedEvent
        {
            AccountId = dto.AccountId,
            Amount = dto.Amount,
            Reason = dto.Reason
        };

        var version = await GetNextVersionAsync(dto.AccountId);
        await AppendEventAsync(dto.AccountId, "MoneyDeposited", dto.Amount, @event, version);

        // update projection
        account.Balance += dto.Amount;
        account.LastUpdated = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return MapToResponseDto(account);
    }

    // ─── Withdraw ─────────────────────────────────────────────────────
    public async Task<AccountResponseDto> WithdrawAsync(WithdrawMoneyDto dto)
    {
        var account = await GetAccountOrThrowAsync(dto.AccountId);

        if (account.Balance < dto.Amount)
            throw new InvalidOperationException($"Insufficient balance. Current balance: {account.Balance}");

        var @event = new MoneyWithdrawnEvent
        {
            AccountId = dto.AccountId,
            Amount = dto.Amount,
            Reason = dto.Reason
        };

        var version = await GetNextVersionAsync(dto.AccountId);
        await AppendEventAsync(dto.AccountId, "MoneyWithdrawn", dto.Amount, @event, version);

        // update projection
        account.Balance -= dto.Amount;
        account.LastUpdated = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return MapToResponseDto(account);
    }

    // ─── Transfer ─────────────────────────────────────────────────────
    public async Task<string> TransferAsync(TransferMoneyDto dto)
    {
        var fromAccount = await GetAccountOrThrowAsync(dto.FromAccountId);
        var toAccount = await GetAccountOrThrowAsync(dto.ToAccountId);

        if (fromAccount.Balance < dto.Amount)
            throw new InvalidOperationException($"Insufficient balance. Current balance: {fromAccount.Balance}");

        var @event = new TransferRecordedEvent
        {
            FromAccountId = dto.FromAccountId,
            ToAccountId = dto.ToAccountId,
            Amount = dto.Amount,
            Reason = dto.Reason
        };

        // append two events — one for each side of the transfer
        var fromVersion = await GetNextVersionAsync(dto.FromAccountId);
        var ledgerEvent = new LedgerEvent
        {
            AccountId = dto.FromAccountId,
            EventType = "TransferOut",
            Amount = dto.Amount,
            Payload = JsonSerializer.Serialize(@event),
            RelatedAccountId = dto.ToAccountId,
            CreatedAt = DateTime.UtcNow,
            Version = fromVersion
        };
        _db.LedgerEvents.Add(ledgerEvent);

        var toVersion = await GetNextVersionAsync(dto.ToAccountId);
        var ledgerEvent2 = new LedgerEvent
        {
            AccountId = dto.ToAccountId,
            EventType = "TransferIn",
            Amount = dto.Amount,
            Payload = JsonSerializer.Serialize(@event),
            RelatedAccountId = dto.FromAccountId,
            CreatedAt = DateTime.UtcNow,
            Version = toVersion
        };
        _db.LedgerEvents.Add(ledgerEvent2);

        // update both projections
        fromAccount.Balance -= dto.Amount;
        fromAccount.LastUpdated = DateTime.UtcNow;

        toAccount.Balance += dto.Amount;
        toAccount.LastUpdated = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return $"Transfer of {dto.Amount} from {fromAccount.OwnerName} to {toAccount.OwnerName} successful.";
    }

    // ─── Get Account ──────────────────────────────────────────────────
    public async Task<AccountResponseDto> GetAccountAsync(string accountId)
    {
        var account = await GetAccountOrThrowAsync(accountId);
        return MapToResponseDto(account);
    }

    // ─── Get All Accounts ─────────────────────────────────────────────
    public async Task<List<AccountResponseDto>> GetAllAccountsAsync()
    {
        var accounts = await _db.AccountBalances.ToListAsync();
        return accounts.Select(MapToResponseDto).ToList();
    }

    // ─── Get Event History ────────────────────────────────────────────
    public async Task<List<LedgerEventResponseDto>> GetEventHistoryAsync(string accountId)
    {
        var events = await _db.LedgerEvents
            .Where(e => e.AccountId == accountId)
            .OrderBy(e => e.Version)
            .ToListAsync();

        return events.Select(e => new LedgerEventResponseDto
        {
            Id = e.Id,
            EventType = e.EventType,
            Amount = e.Amount,
            Payload = e.Payload,
            RelatedAccountId = e.RelatedAccountId,
            CreatedAt = e.CreatedAt,
            Version = e.Version
        }).ToList();
    }

    // ─── Private Helpers ──────────────────────────────────────────────
    private async Task<AccountBalance> GetAccountOrThrowAsync(string accountId)
    {
        var account = await _db.AccountBalances.FindAsync(accountId);
        if (account == null)
            throw new InvalidOperationException($"Account '{accountId}' not found.");
        return account;
    }

    private async Task<int> GetNextVersionAsync(string accountId)
    {
        var lastVersion = await _db.LedgerEvents
            .Where(e => e.AccountId == accountId)
            .MaxAsync(e => (int?)e.Version) ?? 0;
        return lastVersion + 1;
    }

    private async Task AppendEventAsync(string accountId, string eventType, decimal amount, object eventData, int version)
    {
        var ledgerEvent = new LedgerEvent
        {
            AccountId = accountId,
            EventType = eventType,
            Amount = amount,
            Payload = JsonSerializer.Serialize(eventData),
            CreatedAt = DateTime.UtcNow,
            Version = version
        };
        _db.LedgerEvents.Add(ledgerEvent);
    }

    private static AccountResponseDto MapToResponseDto(AccountBalance account) => new()
    {
        AccountId = account.AccountId,
        OwnerName = account.OwnerName,
        Balance = account.Balance,
        CreatedAt = account.CreatedAt,
        LastUpdated = account.LastUpdated
    };
}