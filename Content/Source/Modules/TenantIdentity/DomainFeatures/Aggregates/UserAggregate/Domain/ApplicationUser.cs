﻿using Microsoft.AspNetCore.Identity;
using Modules.TenantIdentity.DomainFeatures.Aggregates.TenantAggregate.Domain;
using Modules.TenantIdentity.DomainFeatures.Aggregates.TenantAggregate.Domain.Exceptions;
using Shared.Kernel.BuildingBlocks.Authorization;

namespace Modules.TenantIdentity.DomainFeatures.Aggregates.UserAggregate.Domain
{
    public class ApplicationUser : IdentityUser<Guid>, IUser
    {
        public string PictureUri { get; set; }
        public bool IsOnline => CountOfOpenTabs > 0;
        public int CountOfOpenTabs { get; set; }
        public Guid SelectedTenantId { get; set; }
        public IList<TenantMembership> TenantMemberships { get; set; }

        public void IncrementOpenTabCount()
        {
            CountOfOpenTabs++;
        }

        public void DecrementOpenTabCount()
        {
            if (CountOfOpenTabs == 0)
            {
                throw new TabsAlreadyClosedException("User has no tabs to close");
            }
            CountOfOpenTabs--;
        }

        public virtual ICollection<IdentityUserLogin<Guid>> Logins { get; set; }
        public virtual ICollection<IdentityUserClaim<Guid>> Claims { get; set; }
        public virtual ICollection<IdentityUserToken<Guid>> Tokens { get; set; }
    }
}