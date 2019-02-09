using System.Threading.Tasks;

namespace AaltoGlobalImpact.OIP
{
    public interface IBeforeStoreHandler
    {
        Task PerformBeforeStoreUpdate();
    }
}