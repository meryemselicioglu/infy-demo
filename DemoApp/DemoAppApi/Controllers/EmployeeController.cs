using DemoAppApi.Interfaces.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoAppApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : Controller
    {
        #region  Members
        private readonly ILogger<EmployeeController> _logger;
        private readonly IDashboardService _dashboardService;
        private readonly IEmployeeInfo _employeeInfoService;

        #endregion

        #region  Constructor
        public EmployeeController(ILogger<EmployeeController> logger,
        IDashboardService dashboardService, IEmployeeInfo employeeInfoService)
        {
            _logger = logger;
            _dashboardService = dashboardService;
            _employeeInfoService = employeeInfoService;
        }

        #endregion

        #region Endpoints
        [HttpPost("dashboard")]
        public async Task<IActionResult> AddNotice([FromBody] Notice request)
        {

            var username = User.Identity?.Name;
            if (username == null)
                return Unauthorized();

            request.Username = username;

            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
            {
                _logger.LogWarning("AddNotice called with missing title or description");
                return BadRequest("Title and content are required");
            }

            try
            {
                var newNotice = await _dashboardService.AddNoticeAsync(request.Title, request.Description, request.Username);
                _logger.LogInformation("Notice created with Id {NoticeId}, Title: {Title}", newNotice.Id, newNotice.Title);
                return Ok(newNotice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while adding a new notice");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetAllNotices()
        {
            var username = User.Identity?.Name;
            if (username == null)
                return Unauthorized();

            try
            {
                var notices = await _dashboardService.GetAllNoticesAsync(username);
                return Ok(notices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all notices");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("dashboard/{id:int}")]
        public async Task<IActionResult> UpdateNotice(int id, [FromBody] Notice request)
        {
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
            {
                _logger.LogWarning("UpdateNotice called with missing description or title");
                return BadRequest("Title and content are required.");
            }

            try
            {
                var updatedNotice = await _dashboardService.UpdateNoticeAsync(id, request.Title, request.Description, request.Status);
                if (updatedNotice == null)
                {
                    _logger.LogWarning("Update failed: Notice with Id {NoticeId} not found", id);
                    return NotFound();
                }

                return Ok(updatedNotice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notice with Id {NoticeId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("dashboard/{id:int}")]
        public async Task<IActionResult> DeleteNotice(int id)
        {
            try
            {
                var result = await _dashboardService.DeleteNoticeAsync(id);
                if (!result)
                {
                    _logger.LogWarning("Delete failed: Notice with Id {NoticeId} not found", id);
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notice with Id {NoticeId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetEmployeeInfo()
        {
            var username = User.Identity?.Name;
            if (username == null)
                return Unauthorized();

            try
            {
                var employee = await _employeeInfoService.GetEmployeeAsync(username);
                if (employee == null)
                {
                    _logger.LogWarning("Get failed: Employee with username {username} not found", username);
                    return NotFound();
                }

                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting the employee info with Username {username}", username);
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion
    }
}