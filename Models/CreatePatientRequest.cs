class CreatePatientRequest
{
    public required string FullName { set; get; }
    public required DateTime DateOfBirth { set; get; }
    public required string Gender { set; get; }
    public required string PhoneNumber { set; get; }
    public required string Email { set; get; }
}