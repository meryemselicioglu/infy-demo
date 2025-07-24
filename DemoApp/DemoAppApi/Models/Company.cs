public class Company
{
    public int Id { get; set; }
    public required string EmployeeId { get; set; }
    public required string Department { get; set; }
    public required string Designation { get; set; }
    public DateTime JoiningDate { get; set; }
    public required string Status { get; set; }
}
