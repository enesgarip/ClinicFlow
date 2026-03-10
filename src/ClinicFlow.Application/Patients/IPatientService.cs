using ClinicFlow.Application.Patients.Dtos;

namespace ClinicFlow.Application.Patients;

public interface IPatientService
{
    Task<IReadOnlyList<PatientDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<PatientDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PatientDto> CreateAsync(CreatePatientDto createPatientDto, CancellationToken cancellationToken);
    Task<PatientDto?> UpdateAsync(Guid id, UpdatePatientDto updatePatientDto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
