using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using app1API.Entities;
using Microsoft.AspNetCore.Authorization;

namespace app1API.Authorization
{
    public class ResourceOperationRequirementHandler: AuthorizationHandler<ResourceOperationRequirement, Restaurant>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOperationRequirement requirement,
            Restaurant restaurant)
        {
            if (requirement.ResourceOperation == ResourceOperationEnum.ResourceOperation.Read ||
                requirement.ResourceOperation == ResourceOperationEnum.ResourceOperation.Create)
            {
                context.Succeed(requirement);
            }

            var userId = context.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (restaurant.CreatedById == int.Parse(userId))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
