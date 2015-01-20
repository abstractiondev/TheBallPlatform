using System;

namespace SQLiteSupport
{
    public class ReflectionSupport
    {
        public static Type GetSQLiteDataContextType(string typeNameInMe)
        {
            return Type.GetType(typeNameInMe);
        }
    }
}