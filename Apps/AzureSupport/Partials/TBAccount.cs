using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheBall;
using TheBall.Core;

namespace AaltoGlobalImpact.OIP
{
    partial class TBAccount : IContainerOwner
    {
        public string ContainerName
        {
            get { return "acc"; }
        }

        public string LocationPrefix
        {
            get { return this.ID; }
        }

        public TBEmail GetAccountEmail(string emailAddress)
        {
            TBEmail result = Emails.CollectionContent.FirstOrDefault(candidate => candidate.EmailAddress == emailAddress);
            if(result == null)
                throw new InvalidDataException("Account does not contain email: " + emailAddress);
            return result;
        }


        public void JoinGroup(TBCollaboratingGroup collaboratingGroup, TBCollaboratorRole role)
        {
            if (this.GroupRoleCollection.CollectionContent.Find(member => member.GroupID == collaboratingGroup.ID) != null)
                return;
            this.GroupRoleCollection.CollectionContent.Add(new TBAccountCollaborationGroup()
                                                               {
                                                                   GroupID = collaboratingGroup.ID,
                                                                   GroupRole = role.Role,
                                                                   RoleStatus = role.RoleStatus
                                                               });
        }

        public static async Task<TBAccount> GetAccountFromEmail(string emailAddress)
        {
            string emailRootID = TBREmailRoot.GetIDFromEmailAddress(emailAddress);
            TBREmailRoot emailRoot = await ObjectStorage.RetrieveFromDefaultLocationA<TBREmailRoot>(emailRootID);
            TBAccount account = emailRoot.Account;
            return account;
        }

    }
}
