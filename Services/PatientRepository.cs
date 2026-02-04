using Microsoft.EntityFrameworkCore;

class PatientRepository : IPatientRepository
{
    private readonly PatientDbContext _context;
    public PatientRepository(PatientDbContext context)
    {
        _context = context;
    }

    public Patient? Add(Patient patient)
    {
        if (patient.FullName == null || patient.Gender == null || patient.PhoneNumber == null || patient.Email == null)
        {
            throw new ArgumentNullException("Patient is invalid");
        }
        _context.Patients.Add(patient);
        _context.SaveChanges();
        return patient;
    }

    public bool Delete(int id)
    {
        Patient? patient = GetById(id);

        if (patient == null)
        {
            return false;
        }
        else
        {
            _context.Patients.Remove(patient);
            _context.SaveChanges();
            return true;
        }

    }

    public IEnumerable<Patient> GetAll()
    {
        return _context.Patients.ToList();
    }

    public Patient? GetById(int id)
    {
        return _context.Patients.FirstOrDefault(p => p.Id == id);
    }

    public Patient? Update(Patient patient)
    {
        Patient? existingPatient = GetById(patient.Id);
        if (existingPatient == null)
        {
            return null;
        }
        existingPatient.FullName = patient.FullName;
        existingPatient.DateOfBirth = patient.DateOfBirth;
        existingPatient.Gender = patient.Gender;
        existingPatient.PhoneNumber = patient.PhoneNumber;
        existingPatient.Email = patient.Email;
        existingPatient.AdmittedAt = patient.AdmittedAt;
        existingPatient.IsDischarged = patient.IsDischarged;
        _context.Patients.Update(existingPatient);
        _context.SaveChanges();
        return existingPatient;
    }

}

