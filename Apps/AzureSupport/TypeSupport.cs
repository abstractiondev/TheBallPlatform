using System;
using System.Reflection;

namespace TheBall.CORE
{
    public static class TypeSupport
    {
        public static Type GetTypeByName(string fullName)
        {
            // TODO: Reflect proper loading based on fulltype, right now fetching from this
            Assembly currAsm = Assembly.GetExecutingAssembly();
            Type type = currAsm.GetType(fullName);
            return type;
        }
    }
}