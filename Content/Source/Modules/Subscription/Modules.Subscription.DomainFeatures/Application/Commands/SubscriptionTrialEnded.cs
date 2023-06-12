﻿using Shared.Infrastructure.CQRS.Command;
using Stripe;

namespace Modules.Subscription.DomainFeatures.Application.Commands
{
    public class SubscriptionTrialEnded : ICommand
    {
        public Subscription Subscription { get; set; }
    }
}
