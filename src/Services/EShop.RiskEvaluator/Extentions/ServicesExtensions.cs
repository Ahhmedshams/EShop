using EShop.RiskEvaluator.Services.Rules;

namespace EShop.RiskEvaluator.Extentions;

public static class ServicesExtensions
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(TimeProvider.System);

        builder.Services.AddSingleton<IRule, AgeRule>();
        builder.Services.AddSingleton<IRule, EmailRule>();
        builder.Services.AddSingleton<IRule, MembershipRule>(sp => new MembershipRule(
            builder.Configuration.GetValue<bool>("Feature:PremiumMembershipFailure")));
        return builder;
    }
}