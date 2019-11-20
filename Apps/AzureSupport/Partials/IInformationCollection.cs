using System.Threading.Tasks;

namespace TheBall.Core
{
    public interface IInformationCollection
    {
        string GetItemDirectory();
        Task RefreshContentAsync();
        bool IsMasterCollection { get; }
        string GetMasterLocation();
        Task<IInformationCollection> GetMasterInstanceAsync();
    }
}