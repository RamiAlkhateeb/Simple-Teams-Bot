
using Clincs.Common.Helpers;
using Common.Helpers.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;


namespace Common.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, IJwtUtils jwtUtils)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var user = jwtUtils.ValidateJwtToken(token);
            if (user != null)
            {
                // attach user to context on successful jwt validation
                context.Items["UserId"] = user.userId;
                context.Items["AadObjectId"] = user.AadObjectId;
                context.Items["Role"] = user.Role;

            }else
                context.Items["Role"] = 99;

            await _next(context);
        }
    }
}