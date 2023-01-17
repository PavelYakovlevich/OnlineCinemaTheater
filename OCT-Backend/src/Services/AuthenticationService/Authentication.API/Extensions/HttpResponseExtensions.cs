namespace Authentication.API.Extensions;

public static class HttpResponseExtensions
{
    public static void AddAuthTokens(this HttpResponse response, string accessToken, string refreshToken)
    {
        response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            SameSite = SameSiteMode.None,
            Secure = true
        });

        response.Cookies.Append("accessToken", accessToken, new CookieOptions
        {
            SameSite = SameSiteMode.None,
            Secure = true
        });
    }
}
