using DemoAppApi.Data;
using Microsoft.EntityFrameworkCore;

namespace DemoAppApi.Services
{
    public class EmployeeInfoService : IEmployeeInfo
    {
        #region Members

        private readonly AppDbContext _dbContext;
        private readonly ILogger<EmployeeInfoService> _logger;

        #endregion

        #region Constructor
        public EmployeeInfoService(AppDbContext dbContext, ILogger<EmployeeInfoService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion

        #region Public Methods

        public async Task<Employee?> GetEmployeeAsync(string username)
        {
            try
            {
                _logger.LogInformation("Fetching employee with username {username}", username);

                var employee = await _dbContext.Employees.
                    FirstOrDefaultAsync(e => e.Username == username);

                if (employee == null)
                {
                    _logger.LogWarning("Employee with username {username} not found", username);
                    return null;
                }

                _logger.LogInformation("Successfully fetched employee {username}", username);
                return employee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occured while fetching the employee");
                throw;
            }
        }

        #endregion
    }
}