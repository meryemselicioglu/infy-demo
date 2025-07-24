using DemoAppApi.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly PasswordHasher<Login> _passwordHasher = new();
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext dbContext, ILogger<AuthService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }


    public async Task<Login?> LoginAsync(string username, string password)
    {
        _logger.LogInformation("Login attempt for username '{Username}'", username);

        var user = await _dbContext.Logins.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            _logger.LogWarning("Login failed: user '{Username}' not found", username);
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Success)
        {
            _logger.LogInformation("Password verification succeeded for user '{Username}'", username);
            return user;
        }

        _logger.LogWarning("Password verification failed for user '{Username}'", username);
        return null;
    }


    public async Task<SignupResult> SignupAsync(string username, string password, string role)
    {
        _logger.LogInformation("Signup attempt for username '{Username}'", username);

        try
        {
            if (role != "admin")
            {
                var employeeExists = await _dbContext.Employees.AnyAsync(e => e.Username == username);
                if (!employeeExists)
                {
                    return SignupResult.EmployeeNotRegistered;
                }
            }

            if (await _dbContext.Logins.AnyAsync(u => u.Username == username))
            {
                return SignupResult.AlreadyExists;
            }

            var user = new Login
            {
                Username = username,
                Role = role
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            _dbContext.Logins.Add(user);
            await _dbContext.SaveChangesAsync();
            return SignupResult.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred during signup for user '{Username}'", username);
            return SignupResult.Error;
        }
    }
}
