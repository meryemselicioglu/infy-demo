public interface IAuthService
{
    Task<Login?> LoginAsync(string username, string password);

    Task<SignupResult> SignupAsync(string username, string password, string role = "emp");

}