
public interface IEmployeeInfo
{
    Task<Employee?> GetEmployeeAsync(string username);
}
