using System;
using System.Threading.Tasks;
using TheBall;
using TheBall.Core;

namespace ProBroz.OnlineTraining
{
    public class SaveMemberImplementation
    {
        public static async Task<Member> GetTarget_MemberToSaveAsync(INT.Member memberData)
        {
            var id = memberData.ID;
            var etag = memberData.ETag;
            var member = await ObjectStorage.RetrieveFromOwnerContentA<Member>(id, etag, true);
            member.FirstName = memberData.FirstName;
            member.LastName = memberData.LastName;
            member.MiddleName = memberData.MiddleName;
            member.BirthDay = memberData.BirthDay;
            member.Email = memberData.Email;
            member.PhoneNumber = memberData.PhoneNumber;
            member.Address = memberData.Address;
            member.Address2 = memberData.Address2;
            member.ZipCode = memberData.ZipCode;
            member.PostOffice = memberData.PostOffice;
            member.Country = memberData.Country;
            member.FederationLicense = memberData.FederationLicense;
            member.PhotoPermission = memberData.PhotoPermission;
            member.VideoPermission = memberData.VideoPermission;

            return member;
        }

        public static async Task ExecuteMethod_StoreObjectAsync(Member memberToSave)
        {
            await memberToSave.StoreInformationAsync(InformationContext.CurrentOwner);
        }
    }
}