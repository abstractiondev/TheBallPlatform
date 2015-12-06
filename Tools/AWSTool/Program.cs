using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AWSTool
{
    class Program
    {
        static void Main(string[] args)
        {
            string accessKey = args[0];
            string secretKey = args[1];
            string domain = args[2];

            AWSManager manager = new AWSManager(accessKey, secretKey);
            manager.InitDomainWithSES(domain);
        }

    }
}
