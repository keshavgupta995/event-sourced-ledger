# Event Sourced Ledger

A double-entry bank core built with ASP.NET Core and MySQL, implementing the Event Sourcing pattern from scratch — without any event store library.

Every financial transaction is stored as an immutable event. Account balances are projections derived from replaying those events. No financial record is ever updated or deleted.

---

## What this project demonstrates

- Event Sourcing implemented manually using MySQL as the event store
- Double-entry accounting — every transaction has two sides that always balance
- CQRS separation — write side appends events, read side queries projections
- Append-only ledger — the complete financial history of any account is always available
- Clean layered architecture — Domain, Services, Controllers, Validators, DTOs

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core Web API (.NET 8) |
| Database | MySQL |
| ORM | Entity Framework Core + Pomelo |
| Validation | FluentValidation |
| API Docs | Scalar (OpenAPI) |
| Frontend | HTML + Tailwind CSS + Vanilla JS |

---

## Architecture

The system is built around two core tables:

**ledger_events** — append-only event store. Records are never updated or deleted.
id, account_id, event_type, amount, payload (JSON), related_account_id, version, created_at

**account_balances** — read model projection. Rebuilt by replaying events.
account_id, owner_name, balance, created_at, last_updated

Every financial action (deposit, withdrawal, transfer) appends one or more events to `ledger_events` and updates the projection in `account_balances`. The projection is always reconstructable from the event history alone.

---

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/accounts` | Open a new account |
| GET | `/api/accounts` | Get all accounts |
| GET | `/api/accounts/{id}` | Get account details and balance |
| GET | `/api/accounts/{id}/history` | Get full event history for an account |
| POST | `/api/transactions/deposit` | Deposit money |
| POST | `/api/transactions/withdraw` | Withdraw money |
| POST | `/api/transactions/transfer` | Transfer between two accounts |

The `/history` endpoint is the core feature — it returns the complete, ordered sequence of events for any account, which can be replayed to reconstruct the balance at any point in time.

---

## Running Locally

**Prerequisites**
- .NET 8 SDK
- MySQL 8.0

**Steps**

1. Clone the repository

```bash
git clone https://github.com/keshavgupta995/event-sourced-ledger.git
cd event-sourced-ledger
```

2. Set up your database connection in `appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=ledger_db;User=root;Password=your_password;"
  }
}
```

3. Apply migrations

```bash
cd LedgerApi
dotnet ef database update
```

4. Run the project

```bash
dotnet run --launch-profile http
```

5. Open your browser at `http://localhost:5070`

---

## Frontend

The frontend is intentionally minimal — plain HTML, Tailwind CSS via CDN, and Vanilla JS. It exists only to demonstrate the system, not to be a full UI.

- Dashboard — live account count, total balance, total events
- Accounts — create accounts, view balances
- Transactions — deposit, withdraw, transfer
- Audit Log — query the full event history of any account

API documentation is available at `http://localhost:5070/scalar/v1`

---

## Project Structure

LedgerApi/

├── Controllers/        — API endpoints

├── Data/

│   └── Models/         — EF Core models (LedgerEvent, AccountBalance)

├── Domain/

│   └── Events/         — Event classes (AccountOpened, MoneyDeposited, etc.)

├── DTOs/               — Request and response shapes

├── Services/           — Business logic (AccountService)

├── Validators/         — FluentValidation rules

└── wwwroot/            — Minimal HTML frontend


---

## Key Design Decisions

**Why Event Sourcing?**
Standard CRUD banking systems overwrite balances. If something goes wrong, the history is gone. This system never overwrites anything — every state change is recorded as a fact, making the entire financial history auditable and replayable.
