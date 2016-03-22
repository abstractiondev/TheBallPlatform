using System.Threading.Tasks;
using TheBall;

namespace ProBroz.OnlineTraining
{
    public class DeleteMemberImplementation
    {
        public static async Task<Member> GetTarget_MemberToDeleteAsync(INT.Member memberData)
        {
            var id = memberData.ID;
            var etag = memberData.ETag;
            var member = await ObjectStorage.RetrieveFromOwnerContentA<Member>(id, etag, true);
            return member;
        }

        public static async Task ExecuteMethod_DeleteObjectAsync(Member memberToDelete)
        {
            await memberToDelete.DeleteInformationObjectAsync(InformationContext.CurrentOwner);
        }
    }
}