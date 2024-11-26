﻿using System.Security.Claims;
using Bit.Api.Tools.Authorization;
using Bit.Core.AdminConsole.OrganizationFeatures.Shared.Authorization;
using Bit.Core.Context;
using Bit.Core.Enums;
using Bit.Core.Models.Data;
using Bit.Core.Test.AdminConsole.Helpers;
using Bit.Test.Common.AutoFixture;
using Bit.Test.Common.AutoFixture.Attributes;
using Microsoft.AspNetCore.Authorization;
using NSubstitute;
using Xunit;

namespace Bit.Api.Test.Tools.Authorization;

[SutProviderCustomize]
public class VaultExportAuthorizationHandlerTests
{
    public static IEnumerable<object[]> CanExportWholeVault => new List<CurrentContextOrganization>
    {
        new () { Type = OrganizationUserType.Owner },
        new () { Type = OrganizationUserType.Admin },
        new ()
        {
            Type = OrganizationUserType.Custom, Permissions = new Permissions { AccessImportExport = true }
        }
    }.Select(org => new[] { org });

    [Theory]
    [BitMemberAutoData(nameof(CanExportWholeVault))]
    public async Task ExportAll_PermittedRoles_Success(CurrentContextOrganization org, OrganizationScope orgScope, ClaimsPrincipal user,
        SutProvider<VaultExportAuthorizationHandler> sutProvider)
    {
        org.Id = orgScope;
        sutProvider.GetDependency<ICurrentContext>().GetOrganization(orgScope).Returns(org);

        var authContext = new AuthorizationHandlerContext(new[] { VaultExportOperations.ExportWholeVault }, user, orgScope);
        await sutProvider.Sut.HandleAsync(authContext);

        Assert.True(authContext.HasSucceeded);
    }

    public static IEnumerable<object[]> CannotExportWholeVault => new List<CurrentContextOrganization>
    {
        new () { Type = OrganizationUserType.User },
        new ()
        {
            Type = OrganizationUserType.Custom, Permissions = new Permissions { AccessImportExport = true }.Invert()
        },
        null    // not a member
    }.Select(org => new[] { org });

    [Theory]
    [BitMemberAutoData(nameof(CannotExportWholeVault))]
    public async Task ExportAll_NotPermitted_Failure(CurrentContextOrganization? org, OrganizationScope orgScope, ClaimsPrincipal user,
        SutProvider<VaultExportAuthorizationHandler> sutProvider)
    {
        if (org is not null)
        {
            org.Id = orgScope;
        }

        sutProvider.GetDependency<ICurrentContext>().GetOrganization(orgScope).Returns(org);

        var authContext = new AuthorizationHandlerContext(new[] { VaultExportOperations.ExportWholeVault }, user, orgScope);
        await sutProvider.Sut.HandleAsync(authContext);

        Assert.False(authContext.HasSucceeded);
    }
}
