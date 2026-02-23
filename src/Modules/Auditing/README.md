# Auditing Module

## Overview

Audit logging and tracking module for recording entity changes and user actions.

## Structure

### Domain Layer (Auditing.Domain)

- **Entities**: AuditLogEntry entity
- **Repositories**: IAuditLogRepository interface

### Application Layer (Auditing.Application)

- **Features**: AuditLogEntries queries
- **DTOs**: AuditLogEntryDto

### Infrastructure Layer (Auditing.Infrastructure)

- **Persistence**: Database context and entity configurations
- **Repositories**: IAuditLogRepository implementation

### API Layer (Auditing.Api)

- **Controllers**: HTTP endpoints for audit logs
- **Extensions**: Module registration

## Dependencies

### Domain

- No external dependencies

### Application

- Auditing.Domain
- BuildingBlocks.Application

### Infrastructure

- Auditing.Application
- Microsoft.EntityFrameworkCore

### API

- Auditing.Infrastructure
- BuildingBlocks.Api
- MediatR

## Key Features

- Audit log entry tracking
- Paginated query of audit entries
- Integration with user actions
- Timestamp tracking

## Usage

Register this module in the main API project:

```csharp
builder.Services.AddAuditingModule();
```

## Notes

- AuditLogEntry tracks who did what and when
- Queries support pagination for large datasets
- Can be extended to support filtering by user, action, or object
