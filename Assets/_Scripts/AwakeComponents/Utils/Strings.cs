using AwakeComponents.DebugUI;

namespace AwakeComponents.Utils
{
    [ComponentInfo("0.1.0", "06.04.2024")]
    public static class Strings
    {
        public static string FirstCharToUpper(string str)
        {
            if (str.Length == 0)
                return "";
            
            if (str.Length == 1)
                return char.ToUpper(str[0]).ToString();
            
            return char.ToUpper(str[0]) + str[1..];
        }
    }
}