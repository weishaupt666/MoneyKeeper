using System.Security.Claims;

namespace MoneyKeeper.Extensions;

public static class ClaimsExtensions
{
    public static int GetUserId(this System.Security.Claims.ClaimsPrincipal user)
    {
        var idClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

        if (idClaim == null || !int.TryParse(idClaim.Value, out int userId))
        {
            throw new UnauthorizedAccessException("User ID is missing or invalid.");
        }

        return userId;
    }
}
