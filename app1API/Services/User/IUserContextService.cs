using System.Security.Claims;

namespace app1API.Services
{
    public interface IUserContextService
    {
        ClaimsPrincipal User { get; }
        int? GetUserId { get; }
    }
}