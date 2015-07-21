using System;

namespace TheBall.Interface
{
    public class InterfaceOperationImplementation
    {
        public static string GetID()
        {
            string id = DateTime.UtcNow.ToString("yyyyMMdd_hhmmss") + "_" + Guid.NewGuid().ToString();
            return id;
        }
    }
}