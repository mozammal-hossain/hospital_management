public class Patient
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public required DateTime DateOfBirth { get; set; }
    public required string Gender { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    public DateTime? AdmittedAt { get; set; }
    public bool? IsDischarged { get; set; }
}