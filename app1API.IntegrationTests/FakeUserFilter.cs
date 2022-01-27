using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace app1API.IntegrationTests
{
    public class FakeUserFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var claimsPrinciple = new ClaimsPrincipal();

            claimsPrinciple.AddIdentities(new[]
            {
                new ClaimsIdentity(
                    new []
                    {
                        new Claim(ClaimTypes.NameIdentifier, "1"),
                        new Claim(ClaimTypes.Role, "Admin")
                    })
            });

            context.HttpContext.User = claimsPrinciple;

            await next();
        }
    }
}
