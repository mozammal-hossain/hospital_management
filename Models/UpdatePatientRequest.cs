public class UpdatePatientRequest
{
    public string? FullName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public DateTime? AdmittedAt { get; set; }
    public bool? IsDischarged { get; set; }
}