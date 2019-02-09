using System;
using System.IO;
using AaltoGlobalImpact.OIP;

namespace TheBall.Payments
{
    public class GrantPaidAccessToGroupImplementation
    {
        public static void ExecuteMethod_AddAccountToGroup(string groupId, string accountId)
        {
            if (accountId != InformationContext.CurrentAccount.AccountID)
                throw new NotSupportedException("Granting access to account is only supported for executing account owners behalf");
            var accountEmail = InformationContext.CurrentAccount.AccountEmail;
            if (string.IsNullOrEmpty(accountEmail))
                throw new InvalidDataException("Account email is required for granting access to account for plan groups");
            throw new NotImplementedException();
       }
    }
}