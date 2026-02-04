public class CreatePatientRequest
{
    public required string FullName { get; set; }
    public required DateTime DateOfBirth { set; get; }
    public required string Gender { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
}