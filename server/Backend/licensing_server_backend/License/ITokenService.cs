
namespace Licensing.License
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(string keyId, string email);
        Task<string> ValidateJwt(string jwtToken);
    }
}