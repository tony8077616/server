﻿using System.Security.Claims;
using Bit.Core.AdminConsole.OrganizationFeatures.Shared.Authorization;
using Bit.Core.Context;
using Bit.Core.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Bit.Core.Tools.Authorization;

public class VaultExportAuthorizationHandler(ICurrentContext currentContext)
    : AuthorizationHandler<VaultExportOperationRequirement, OrganizationScope>
{

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        VaultExportOperationRequirement requirement, OrganizationScope organizationScope)
    {
        var org = currentContext.GetOrganization(organizationScope);

        var authorized = requirement switch
        {
            not null when requirement.Name == nameof(VaultExportOperations.ExportAll) =>
                CanExportAll(org),
            not null when requirement.Name == nameof(VaultExportOperations.ExportPartial) =>
                CanExportPartial(org),
            _ => false
        };

        if (requirement is not null && authorized)
        {
            context.Succeed(requirement);
        }

        return Task.FromResult(0);
    }

    private bool CanExportAll(CurrentContextOrganization organization) => organization is
        { Type: OrganizationUserType.Owner or OrganizationUserType.Admin } or
        { Type: OrganizationUserType.Custom, Permissions.AccessImportExport: true };

    private bool CanExportPartial(CurrentContextOrganization organization) => organization is not null;
}
