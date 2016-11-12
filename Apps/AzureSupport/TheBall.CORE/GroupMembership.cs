using System;

namespace TheBall.CORE
{
    partial class GroupMembership
    {
        public static string GetIDFromAccountAndGroup(string accountId, string groupId)
        {
            return accountId + "_" + groupId;
        }
    }
}