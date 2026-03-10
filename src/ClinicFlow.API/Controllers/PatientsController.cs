using ClinicFlow.Application.Patients;
using ClinicFlow.Application.Patients.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PatientDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var patients = await _patientService.GetAllAsync(cancellationToken);
        return Ok(patients);
    }

    [HttpGet("{id:guid}", Name = "GetPatientById")]
    public async Task<ActionResult<PatientDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var patient = await _patientService.GetByIdAsync(id, cancellationToken);

        if (patient is null)
        {
            return NotFound();
        }

        return Ok(patient);
    }

    [HttpPost]
    public async Task<ActionResult<PatientDto>> CreateAsync([FromBody] CreatePatientDto createPatientDto, CancellationToken cancellationToken)
    {
        if (createPatientDto is null)
        {
            return BadRequest();
        }

        var patient = await _patientService.CreateAsync(createPatientDto, cancellationToken);
        return CreatedAtRoute("GetPatientById", new { id = patient.Id }, patient);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PatientDto>> UpdateAsync(Guid id, [FromBody] UpdatePatientDto updatePatientDto, CancellationToken cancellationToken)
    {
        if (updatePatientDto is null)
        {
            return BadRequest();
        }

        var patient = await _patientService.UpdateAsync(id, updatePatientDto, cancellationToken);

        if (patient is null)
        {
            return NotFound();
        }

        return Ok(patient);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _patientService.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }
}
