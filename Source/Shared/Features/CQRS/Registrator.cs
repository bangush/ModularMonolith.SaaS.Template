﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using Shared.Features.CQRS.Query;
using Shared.Features.CQRS.Command;
using Shared.Features.CQRS.IntegrationEvent;
using Shared.Features.CQRS.DomainEvent;

namespace Shared.Features.CQRS
{
    public static class Registrator
    {
        public static IServiceCollection AddCQRS(this IServiceCollection services, Assembly[] assemblies)
        {
            services.TryAddScoped<ICommandDispatcher, CommandDispatcher>();
            services.TryAddScoped<IQueryDispatcher, QueryDispatcher>();
            services.TryAddScoped<IIntegrationEventDispatcher, IntegrationEventDispatcher>();
            services.TryAddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            // INFO: Using https://www.nuget.org/packages/Scrutor for registering all Query and Command handlers by convention
            services.Scan(selector =>
            {
                selector.FromAssemblies(assemblies)
                        .AddClasses(filter =>
                        {
                            filter.AssignableTo(typeof(IQueryHandler<,>));
                        })
                        .AsImplementedInterfaces()
                        .WithScopedLifetime();
            });
            services.Scan(selector =>
            {
                selector.FromAssemblies(assemblies)
                        .AddClasses(filter =>
                        {
                            filter.AssignableTo(typeof(ICommandHandler<>));
                        })
                        .AsImplementedInterfaces()
                        .WithScopedLifetime();
            });
            services.Scan(selector =>
            {
                selector.FromAssemblies(assemblies)
                        .AddClasses(filter =>
                        {
                            filter.AssignableTo(typeof(IDomainEventHandler<>));
                        })
                        .AsImplementedInterfaces()
                        .WithScopedLifetime();
            });
            services.Scan(selector =>
            {
                selector.FromAssemblies(assemblies)
                        .AddClasses(filter =>
                        {
                            filter.AssignableTo(typeof(IIntegrationEventHandler<>));
                        })
                        .AsImplementedInterfaces()
                        .WithScopedLifetime();
            });
            return services;
        }
    }
}
