using DemoAppApi.Data;
using Microsoft.EntityFrameworkCore;

namespace DemoAppApi.Services
{
    public class AdminService : IAdminService
    {
        #region Members

        private readonly AppDbContext _dbContext;
        private readonly ILogger<AdminService> _logger;

        #endregion

        #region Constructor

        public AdminService(AppDbContext dbContext, ILogger<AdminService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion

        #region Public Methods

        public async Task<Employee?> AddEmployeeAysnc(Employee employee)
        {
            if (await _dbContext.Employees.AnyAsync(e => e.Username == employee.Username))
            {
                _logger.LogWarning("An employee with this username already exists.");
                return null;
            }

            try
            {
                _dbContext.Employees.Add(employee);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Filed to add a new employee");
                throw;
            }
            return employee;
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all employees from database");
                return await _dbContext.Employees.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving all employees");
                throw;
            }
        }

        #endregion
    }
}