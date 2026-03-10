using ClinicFlow.Application.Abstractions;
using ClinicFlow.Application.Patients;
using ClinicFlow.Application.Patients.Dtos;
using ClinicFlow.Domain.Patients;
using ClinicFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Services.Patients;

public class PatientService : IPatientService
{
    private readonly ClinicFlowDbContext _dbContext;
    private readonly ITenantProvider _tenantProvider;

    public PatientService(ClinicFlowDbContext dbContext, ITenantProvider tenantProvider)
    {
        _dbContext = dbContext;
        _tenantProvider = tenantProvider;
    }

    public async Task<IReadOnlyList<PatientDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.GetTenantId();

        return await _dbContext.Patients
            .AsNoTracking()
            .Where(patient => patient.TenantId == tenantId)
            .OrderBy(patient => patient.LastName)
            .ThenBy(patient => patient.FirstName)
            .Select(patient => MapToDto(patient))
            .ToListAsync(cancellationToken);
    }

    public async Task<PatientDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.GetTenantId();

        return await _dbContext.Patients
            .AsNoTracking()
            .Where(patient => patient.TenantId == tenantId && patient.Id == id)
            .Select(patient => MapToDto(patient))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<PatientDto> CreateAsync(CreatePatientDto createPatientDto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(createPatientDto);

        var tenantId = _tenantProvider.GetTenantId();
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            FirstName = createPatientDto.FirstName,
            LastName = createPatientDto.LastName,
            BirthDate = createPatientDto.BirthDate,
            PhoneNumber = createPatientDto.PhoneNumber,
            Email = createPatientDto.Email,
            Notes = createPatientDto.Notes,
            IsActive = createPatientDto.IsActive,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _dbContext.Patients.AddAsync(patient, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(patient);
    }

    public async Task<PatientDto?> UpdateAsync(Guid id, UpdatePatientDto updatePatientDto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(updatePatientDto);

        var tenantId = _tenantProvider.GetTenantId();
        var patient = await _dbContext.Patients
            .SingleOrDefaultAsync(
                existingPatient => existingPatient.TenantId == tenantId && existingPatient.Id == id,
                cancellationToken);

        if (patient is null)
        {
            return null;
        }

        patient.FirstName = updatePatientDto.FirstName;
        patient.LastName = updatePatientDto.LastName;
        patient.BirthDate = updatePatientDto.BirthDate;
        patient.PhoneNumber = updatePatientDto.PhoneNumber;
        patient.Email = updatePatientDto.Email;
        patient.Notes = updatePatientDto.Notes;
        patient.IsActive = updatePatientDto.IsActive;
        patient.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(patient);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.GetTenantId();
        var patient = await _dbContext.Patients
            .SingleOrDefaultAsync(
                existingPatient => existingPatient.TenantId == tenantId && existingPatient.Id == id,
                cancellationToken);

        if (patient is null)
        {
            return false;
        }

        _dbContext.Patients.Remove(patient);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static PatientDto MapToDto(Patient patient)
    {
        return new PatientDto
        {
            Id = patient.Id,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            BirthDate = patient.BirthDate,
            PhoneNumber = patient.PhoneNumber,
            Email = patient.Email,
            Notes = patient.Notes,
            IsActive = patient.IsActive,
            CreatedAtUtc = patient.CreatedAtUtc,
            UpdatedAtUtc = patient.UpdatedAtUtc
        };
    }
}
