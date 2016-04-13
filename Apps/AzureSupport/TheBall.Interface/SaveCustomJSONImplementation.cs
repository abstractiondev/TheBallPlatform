using TheBall.Interface.INT;

namespace TheBall.Interface
{
    public class SaveCustomJSONImplementation
    {
        public static string GetTarget_DataName(CustomJSONData saveDataInfo)
        {
            return saveDataInfo.Name;
        }
    }
}