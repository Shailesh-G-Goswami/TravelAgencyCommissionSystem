# Travel Agency Commission System - Notes

## Assumptions

### Commission Tier Calculation

The commission rate is determined using the agent's cumulative confirmed monthly sales up to and including the current booking date.

Future bookings within the same month do not affect commissions already reserved for earlier bookings. This approach was chosen to ensure deterministic and auditable financial calculations.

If a booking causes the agent to move into a higher commission tier, the higher tier rate is applied to the entire booking amount rather than splitting the commission across multiple tiers. This approach was chosen because it is simpler, easier to understand, and produces deterministic results.

### Repeated Reservation Requests

Only one active commission reservation is allowed per booking.

If a reservation already exists, the API returns HTTP 409 Conflict instead of behaving idempotently. This makes duplicate submissions explicit and prevents accidental double processing.

### Voiding Reservations

When a booking is cancelled, any existing commission reservation is marked as voided by setting:

* `IsVoided = true`
* `VoidedAt = DateTime.UtcNow`

Historical reservation records are preserved for auditing purposes.

### Most Frequently Applied Commission Tier

If multiple commission rates occur with the same frequency for an agent within a month, the lower rate is selected as the most frequent tier. This ensures deterministic and predictable behavior.

### Tier Override Rate

A nullable `TierOverrideRate` is treated differently from an explicit value of `0`.

* `null` means standard tier calculation should be used.
* `0` means a valid override exists and a 0% commission must be applied.

---

## Concurrency Strategy

Commission reservation operations use explicit database transactions to guarantee consistency.

The reservation process:

1. Starts a database transaction.
2. Loads the booking and related data.
3. Validates business rules.
4. Checks whether an active reservation already exists.
5. Creates the reservation.
6. Commits the transaction.

This approach helps prevent duplicate reservations and ensures that cancellation and reservation updates remain atomic.

Cancellation operations also execute inside the same transaction so that both the booking status update and reservation voiding occur together.

If concurrent modifications occur, the user interface displays meaningful feedback and instructs the user to refresh and retry.

---

## Performance Considerations

The commission summary report was designed with scalability in mind.

Implemented optimizations include:

* Server-side pagination.
* Server-side sorting.
* Database-side aggregation using Entity Framework queries.
* Filtering by month before performing calculations.

These approaches are intended to support datasets containing hundreds of thousands of booking records.

---

## Testing Strategy

Unit tests cover:

* Commission tier boundaries.
* Tier override scenarios.
* Cancelled booking restrictions.
* Duplicate reservation behavior.
* Reservation voiding logic.

EF Core InMemory was used for simplicity and fast execution.

A future enhancement would replace transaction-sensitive tests with SQLite in-memory or SQL Server integration tests to more accurately reproduce real database behavior.

---

## Improvements With Additional Time

If additional development time were available, the following improvements would be implemented:

* Global exception handling middleware.
* Full integration tests using SQL Server or SQLite.
* Expanded optimistic concurrency coverage across additional entities and API endpoints.
* Better loading and success notifications in the UI using Bootstrap alerts instead of browser dialogs.
* Search and filtering capabilities on the booking screen.
* Caching for frequently requested monthly reports.
* Authentication and authorization for agent-specific access.
* API versioning and OpenAPI documentation.
* Docker support for simplified deployment.
* CI/CD pipelines for automated builds, tests, and deployments.
