using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace PermissionTest.API.Auth;

public class PermissionsAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions _options;

    public PermissionsAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
    {
        _options = options.Value;
    }

    public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        if (Enum.TryParse(typeof(Permissions), policyName, out var permission))
        {
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new HasPermissionRequirement((Permissions)permission))
                .Build();

            _options.AddPolicy(policyName, policy);
            return Task.FromResult(policy);
        }

        return base.GetPolicyAsync(policyName);
    }

}