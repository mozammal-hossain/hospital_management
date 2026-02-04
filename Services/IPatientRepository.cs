public interface IPatientRepository
{
    public IEnumerable<Patient> GetAll();
    public Patient? GetById(int id);
    public Patient? Add(Patient patient);
    public Patient? Update(Patient patient);
    public bool Delete(int id);
}