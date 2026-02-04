using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/patients")]
public class PatientsController : ControllerBase
{

    private readonly IPatientRepository _repository;

    public PatientsController(IPatientRepository repository)
    {
        _repository = repository;
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
            return NotFound();
        }
        return Ok(patient);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreatePatientRequest request)
    {
        if (request.FullName == null || request.Gender == null || request.PhoneNumber == null || request.Email == null)
        {
            return BadRequest();
        }

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
            return StatusCode(500, "Internal server error");
        }
        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdatePatientRequest request)
    {
        Patient? patient = _repository.GetById(id);

        if (patient == null)
        {
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
            return StatusCode(500, "Internal server error");
        }

        return Ok(updatedPatient);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (_repository.Delete(id))
        {
            return NoContent();
        }
        else
        {
            return NotFound();
        }
    }
}
