# ClinicFlow AI Agent Guide

## Project
ClinicFlow is a multi-tenant clinic management backend.

## Architecture
- .NET 8
- Clean Architecture
- PostgreSQL
- EF Core

## Layers
- Domain
- Application
- Infrastructure
- API

## Rules
- Controllers must remain thin.
- Business logic belongs in services.
- All patient-related operations must be tenant-aware.
- TenantId must be resolved via ITenantProvider.
- Never access tenant data without filtering by TenantId.
- Do not modify unrelated files.
- Follow the existing project structure.

## Validation
- Ensure the solution builds after changes.
- If persistence is modified, ensure migrations still work.