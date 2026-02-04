
class PatientRepository : IPatientRepository
{
    private readonly Dictionary<int, Patient> _patients = new();
    private int _nextId = 1;

    public Patient? Add(Patient patient)
    {
        if (patient.FullName == null | patient.Gender == null || patient.PhoneNumber == null || patient.Email == null)
        {
            throw new ArgumentNullException("Patient is invalid");
        }
        patient.Id = _nextId++;
        _patients.Add(patient.Id, patient);
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
            _patients.Remove(id);
            return true;
        }

    }

    public IEnumerable<Patient> GetAll()
    {
        return _patients.Values;
    }

    public Patient? GetById(int id)
    {
        return _patients.TryGetValue(id, out var _) ? _patients[id] : null;
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
        _patients[existingPatient.Id] = existingPatient;
        return existingPatient;
    }

}

