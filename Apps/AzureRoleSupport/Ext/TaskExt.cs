using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace TheBall.Infra.AzureRoleSupport
{
    internal static class TaskExt
    {
        public static Task AsAwaitable(this CancellationToken token)
        {
            var ev = new AsyncManualResetEvent();
            token.Register(() => ev.Set());
            return ev.WaitAsync();
        }
    }

}
