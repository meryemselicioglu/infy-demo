public interface IAdminService
{
    Task<Employee?> AddEmployeeAysnc(Employee employee);

    Task<List<Employee>> GetAllEmployeesAsync();
}
