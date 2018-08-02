using System.Linq;
using System.Threading.Tasks;
using Footvoter.Services.INT;
using TheBall;

namespace Footvoter.Services
{
    public class DoVoteImplementation
    {
        public static async Task<VotingSummary> GetTarget_UserVotedSummaryAsync()
        {
            var votingSummary = await ObjectStorage.GetInterfaceObject<VotingSummary>();
            return votingSummary;
        }

        public static void ExecuteMethod_PerformVoting(VoteData parametersVoteData, VotingSummary userVotedSummary)
        {
            var voteStatus = parametersVoteData.Votes.Select(vote =>
            {
                var existingVote =
                    userVotedSummary.VotedEntries.FirstOrDefault(item => item.VotedForID == vote.companyID);
                var lastVotedTime = existingVote?.VoteTime;
                return new
                {
                    VotedForID = existingVote?.VotedForID ?? vote.companyID,
                    VoteTime = lastVotedTime
                };
            });

        }

        public static async Task ExecuteMethod_StoreObjectsAsync(VotingSummary userVotedSummary)
        {
            await ObjectStorage.StoreInterfaceObject(InformationContext.CurrentOwner, userVotedSummary);
        }
    }
}