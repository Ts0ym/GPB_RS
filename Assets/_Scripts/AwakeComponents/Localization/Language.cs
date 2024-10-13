using System;

namespace AwakeComponents.Localization
{
    [Serializable]
    public class Language
    {
        public string code;

        public override string ToString()
        {
            return code;
        }
    }
}