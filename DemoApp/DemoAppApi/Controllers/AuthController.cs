using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ITokenService tokenService,
        ILogger<AuthController> logger)
    {
        _logger = logger;
        _authService = authService;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.PasswordHash))
        {
            _logger.LogWarning("Login attempt failed: missing username or password");
            return BadRequest("Invalid login request");
        }

        try
        {
            var user = await _authService.LoginAsync(request.Username, request.PasswordHash);
            if (user != null)
            {
                var token = _tokenService.GenerateToken(user);
                _logger.LogInformation("User '{Username}' logged in successfully", request.Username);
                return Ok(new { token, role = user.Role, message = "Login successful" });
            }

            _logger.LogWarning("Login failed for username '{Username}'", request.Username);
            return Unauthorized(new { message = "Login failed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during login for user '{Username}'", request.Username);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] Login request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.PasswordHash))
        {
            _logger.LogWarning("Signup attempt failed: missing username or password");
            return BadRequest(new { message = "Invalid signup request" });
        }

        try
        {
            var result = await _authService.SignupAsync(request.Username, request.PasswordHash);

            switch (result)
            {
                case SignupResult.Success:
                    _logger.LogInformation("New user '{Username}' signed up successfully", request.Username);
                    return Ok(new { message = "Signup successful" });

                case SignupResult.EmployeeNotRegistered:
                    _logger.LogWarning("Signup failed: username '{Username}' was not pre-registered by admin", request.Username);
                    return BadRequest(new { message = "This username is not registered. Please contact administrator." });

                case SignupResult.AlreadyExists:
                    _logger.LogWarning("Signup failed: username '{Username}' already has an account", request.Username);
                    return Conflict(new { message = "Username already has an account" });

                default:
                    _logger.LogError("Unexpected signup result for '{Username}'", request.Username);
                    return StatusCode(500, new { message = "Internal server error" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during signup for user '{Username}'", request.Username);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}