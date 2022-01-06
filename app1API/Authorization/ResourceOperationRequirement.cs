using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace app1API.Authorization
{
    public class ResourceOperationRequirement: IAuthorizationRequirement
    {
        public ResourceOperationEnum.ResourceOperation ResourceOperation { get; }

        public ResourceOperationRequirement(ResourceOperationEnum.ResourceOperation resourceOperation)
        {
            ResourceOperation = resourceOperation;
        }
    }
}
