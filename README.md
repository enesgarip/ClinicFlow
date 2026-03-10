# ClinicFlow

ClinicFlow is a multi-tenant clinic management backend.

## Tech Stack
- .NET 8
- EF Core
- PostgreSQL

## Architecture
- Domain
- Application
- Infrastructure
- API

## Current Modules
- Tenants
- Patients

## Multi-tenancy
Requests currently use the `X-Tenant-Id` header.
Tenant resolution is handled through `ITenantProvider`.