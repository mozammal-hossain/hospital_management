using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/patients")]
public class PatientsController : ControllerBase
{

    private readonly IPatientRepository _repository;
    private readonly ILogger<PatientsController> _logger;

    public PatientsController(IPatientRepository repository, ILogger<PatientsController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<Patient> GetAll()
    {
        return _repository.GetAll();
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        Patient? patient = _repository.GetById(id);
        if (patient == null)
        {
            _logger.LogWarning("Patient with id {PatientId} not found", id);
            return NotFound();
        }
        _logger.LogInformation("Patient with id {PatientId} found", id);
        return Ok(patient);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreatePatientRequest request)
    {

        Patient patient = new Patient
        {
            FullName = request.FullName,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            AdmittedAt = DateTime.UtcNow,
            IsDischarged = false,
        };

        Patient? createdPatient = _repository.Add(patient);
        if (createdPatient == null)
        {
            _logger.LogError("Failed to create patient");
            return StatusCode(500, "Internal server error");
        }
        _logger.LogInformation("Created patient with id {PatientId}", createdPatient.Id);
        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdatePatientRequest request)
    {
        Patient? patient = _repository.GetById(id);

        if (patient == null)
        {
            _logger.LogWarning("Update failed: patient with id {PatientId} not found", id);
            return NotFound();
        }

        patient.FullName = request.FullName ?? patient.FullName;
        patient.DateOfBirth = request.DateOfBirth ?? patient.DateOfBirth;
        patient.Gender = request.Gender ?? patient.Gender;
        patient.PhoneNumber = request.PhoneNumber ?? patient.PhoneNumber;
        patient.Email = request.Email ?? patient.Email;
        patient.AdmittedAt = request.AdmittedAt ?? patient.AdmittedAt;
        patient.IsDischarged = request.IsDischarged ?? patient.IsDischarged;
        Patient? updatedPatient = _repository.Update(patient);

        if (updatedPatient == null)
        {
            _logger.LogError("Failed to update patient with id {PatientId}", id);
            return StatusCode(500, "Internal server error");
        }

        return Ok(updatedPatient);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (_repository.Delete(id))
        {
            _logger.LogInformation("Deleted patient with id {PatientId}", id);
            return NoContent();
        }
        else
        {
            _logger.LogWarning("Delete failed: patient with id {PatientId} not found", id);
            return NotFound();
        }
    }
}
