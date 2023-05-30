﻿using Modules.TenantIdentity.DomainFeatures.UserAggregate.Domain;
using Shared.Infrastructure.CQRS.Query;
using System.Security.Claims;

namespace Modules.TenantIdentity.DomainFeatures.Application.Queries
{
    public class ClaimsForUserQuery : IQuery<IEnumerable<Claim>>
    {
        public ApplicationUser User { get; set; }
    }
}
