interface IPatientRepository
{
    IEnumerable<Patient> GetAll();
    Patient? GetById(int id);
    Patient? Add(Patient patient);
    Patient? Update(Patient patient);
    bool Delete(int id);
}