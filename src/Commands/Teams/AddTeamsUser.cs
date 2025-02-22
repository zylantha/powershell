﻿using PnP.PowerShell.Commands.Attributes;
using PnP.PowerShell.Commands.Base;
using PnP.PowerShell.Commands.Base.PipeBinds;
using PnP.PowerShell.Commands.Model.Graph;
using PnP.PowerShell.Commands.Utilities;
using System.Management.Automation;

namespace PnP.PowerShell.Commands.Graph
{
    [Cmdlet(VerbsCommon.Add, "PnPTeamsUser")]
    [RequiredMinimalApiPermissions("Group.ReadWrite.All")]
    public class AddTeamsUser : PnPGraphCmdlet
    {
        const string ParamSet_ByUser = "By User";
        const string ParamSet_ByMultipleUsers = "By Multiple Users";

        [Parameter(Mandatory = true, ParameterSetName = ParamSet_ByUser)]
        [Parameter(Mandatory = true, ParameterSetName = ParamSet_ByMultipleUsers)]
        public TeamsTeamPipeBind Team;

        [Parameter(Mandatory = true, ParameterSetName = ParamSet_ByUser)]
        public string User;

        [Parameter(Mandatory = true, ParameterSetName = ParamSet_ByMultipleUsers)]
        public string[] Users;

        [Parameter(Mandatory = true, ParameterSetName = ParamSet_ByUser)]
        [Parameter(Mandatory = true, ParameterSetName = ParamSet_ByMultipleUsers)]
        [ValidateSet(new[] { "Owner", "Member" })]
        public string Role;
        protected override void ExecuteCmdlet()
        {
            var groupId = Team.GetGroupId(HttpClient, AccessToken);
            if (groupId != null)
            {
                try
                {
                    if (ParameterSetName == ParamSet_ByUser)
                    {
                        TeamsUtility.AddUserAsync(HttpClient, AccessToken, groupId, User, Role).GetAwaiter().GetResult();
                    }
                    else
                    {
                        TeamsUtility.AddUsersAsync(HttpClient, AccessToken, groupId, Users, Role).GetAwaiter().GetResult();
                    }

                }
                catch (GraphException ex)
                {
                    if (ex.Error != null)
                    {
                        throw new PSInvalidOperationException(ex.Error.Message);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                throw new PSArgumentException("Group not found");
            }

        }
    }
}