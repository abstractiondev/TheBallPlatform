using System;
using System.Runtime.Remoting.Contexts;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TheBall;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;

namespace AaltoGlobalImpact.OIP
{
    partial class TBRLoginRoot
    {
        public static async Task<TBRLoginRoot> GetOrCreateLoginRootWithAccount(string loginUrl, bool isAccountRequest, string currentDomainName)
        {
            string loginRootID = TBLoginInfo.GetLoginIDFromLoginURL(loginUrl);
            var loginRoot = await ObjectStorage.RetrieveFromDefaultLocationA<TBRLoginRoot>(loginRootID);
            if (loginRoot == null)
            {
                // Storing loginroot
                loginRoot = TBRLoginRoot.CreateDefault();
                loginRoot.ID = loginRootID;
                loginRoot.UpdateRelativeLocationFromID();
                loginRoot.DomainName = currentDomainName;
                await StorageSupport.StoreInformationAsync(loginRoot);

                // Creating login info for account and storing the account
                TBLoginInfo loginInfo = TBLoginInfo.CreateDefault();
                loginInfo.OpenIDUrl = loginUrl;

                TBRAccountRoot accountRoot = TBRAccountRoot.CreateAndStoreNewAccount();
                accountRoot.Account.Logins.CollectionContent.Add(loginInfo);
                string accountID = accountRoot.ID;
                accountRoot.StoreInformation();

                GenericPrincipal principal = (GenericPrincipal) HttpContext.Current.User;
                TheBallIdentity identity = (TheBallIdentity) principal.Identity;
                string emailAddress = identity.EmailAddress;
                if (emailAddress != null)
                {
                    try
                    {

                        RegisterEmailAddress.Execute(new RegisterEmailAddressParameters
                        {
                            AccountID = accountID,
                            EmailAddress = emailAddress
                        });
                        if (InstanceConfig.Current.PlatformDefaultGroupIDList != null)
                        {
                            foreach (var groupToJoinID in InstanceConfig.Current.PlatformDefaultGroupIDList)
                            {
                                try
                                {
                                    JoinAccountToGroup.Execute(new JoinAccountToGroupParameters
                                    {
                                        GroupID = groupToJoinID,
                                        AccountEmailAddress = emailAddress,
                                        MemberRole = TBCollaboratorRole.ViewerRoleValue
                                    });
                                }
                                catch
                                {
                                    
                                }
                                
                            }
                        }
                    }
                    catch (Exception)
                    {

                        UpdateAccountRootToReferences.Execute(new UpdateAccountRootToReferencesParameters
                        {
                            AccountID = accountID
                        });
                        UpdateAccountContainerFromAccountRoot.Execute(new UpdateAccountContainerFromAccountRootParameters
                        {
                            AccountID = accountID
                        });
                    }
                }
                else
                {
                    UpdateAccountRootToReferences.Execute(new UpdateAccountRootToReferencesParameters
                    {
                        AccountID = accountID
                    });
                    UpdateAccountContainerFromAccountRoot.Execute(new UpdateAccountContainerFromAccountRootParameters
                    {
                        AccountID = accountID
                    });
                }

                // If this request is for account, we propagate the pages immediately
                //RenderWebSupport.RefreshAccountTemplates(accountRoot.ID, useBackgroundWorker);
                foreach (var templateName in InstanceConfig.Current.DefaultAccountTemplateList)
                {
                    await RenderWebSupport.RefreshAccountTemplateA(accountID, templateName);
                }
                accountRoot.Account.InitializeAndConnectMastersAndCollections();
                loginRoot = await ObjectStorage.RetrieveFromDefaultLocationA<TBRLoginRoot>(loginRootID);
            }
            return loginRoot;
        }

    }
}