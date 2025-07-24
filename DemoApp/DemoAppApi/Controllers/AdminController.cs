using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DemoAppApi.Controllers
{
    [ApiController]
    [Route("api/admin/employee")]
    public class AdminController : Controller
    {
        #region  Members
        private readonly ILogger<AdminController> _logger;
        private readonly IAdminService _adminService;

        #endregion

        #region  Constructor

        public AdminController(ILogger<AdminController> logger, IAdminService adminService)
        {
            _logger = logger;
            _adminService = adminService;
        }

        #endregion

        #region Endpoints

        [Authorize(Roles = "admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
        {
            if (string.IsNullOrWhiteSpace(employee.Username))
            {
                _logger.LogWarning("AddEmployee called with missing username");
                return BadRequest("Username is required");
            }
            try
            {
                var newEmployee = await _adminService.AddEmployeeAysnc(employee);
                _logger.LogInformation("New employee added with Username {Username}", employee.Username);
                return Ok(newEmployee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while adding a new employee");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllEmployees() 
        {
            try
            {
                var employees = await _adminService.GetAllEmployeesAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all employees");
                return StatusCode(500, "Internal server error");
            }
        }

        #endregion
    }
}