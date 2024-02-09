﻿using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.TenantIdentity.Features.Infrastructure.EFCore;
using Shared.Kernel.BuildingBlocks.Auth;
using Shared.Kernel.BuildingBlocks;
using Shared.Kernel.Extensions.ClaimsPrincipal;
using System;
using System.Linq;
using System.Threading.Tasks;
using Modules.Subscription.Features.Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Web.Server.BuildingBlocks.ServerExecutionContext
{
    public class ServerExecutionContext : IExecutionContext
    {
        private static ServerExecutionContext executionContext;
        private ServerExecutionContext() { }

        public bool AuthenticatedRequest { get; private set; }
        public Guid UserId { get; private set; }
        public Guid TenantId { get; private set; }
        public SubscriptionPlanType TenantPlan { get; private set; }
        public TenantRole TenantRole { get; private set; }
        public IHostEnvironment HostingEnvironment { get; set; }
        public Uri BaseURI { get; private set; }

        public TenantIdentityDbContext TenantIdentityDbContext { get; set; }
        public SubscriptionsDbContext SubscriptionsDbContext { get; set; }

        public static ServerExecutionContext CreateInstance(IServiceProvider serviceProvider)
        {
            if (executionContext is not null)
            {
                return executionContext;
            }

            return new ServerExecutionContext()
            {
                HostingEnvironment = serviceProvider.GetRequiredService<IHostEnvironment>()
            };
        }

        public void InitializeInstance(HttpContext httpContext)
        {
            var server = httpContext.RequestServices.GetRequiredService<IServer>();
            var addresses = server?.Features.Get<IServerAddressesFeature>();

            BaseURI = new Uri(addresses?.Addresses.FirstOrDefault(a => a.Contains("https")) ?? string.Empty);
            TenantIdentityDbContext = httpContext.RequestServices.GetRequiredService<TenantIdentityDbContext>();

            if (httpContext.User.Identity.IsAuthenticated is false)
            {
                AuthenticatedRequest = false;
                return;
            }

            UserId = httpContext.User.GetUserId<Guid>();
            TenantId = httpContext.User.GetTenantId<Guid>();
            TenantPlan = httpContext.User.GetTenantSubscriptionPlanType();
            TenantRole = httpContext.User.GetTenantRole();
        }

        public async Task CommitChangesAsync()
        {
            using var trans = TenantIdentityDbContext.Database.BeginTransaction();
            await SubscriptionsDbContext.Database.UseTransactionAsync(trans.GetDbTransaction());

            await TenantIdentityDbContext.SaveChangesAsync();
            await SubscriptionsDbContext.SaveChangesAsync();

            await trans.CommitAsync();
        }
    }
}
