using System;

namespace AwakeComponents.DebugUI
{
    // Attribute to store the version and last modified date of a component
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentInfoAttribute : Attribute
    {
        public string Version { get; }
        public DateTime LastModifiedDate { get; }

        public ComponentInfoAttribute(string version, string lastModifiedDate)
        {
            Version = version;

            // Используем формат dd.MM.yyyy
            if (!DateTime.TryParseExact(lastModifiedDate, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                throw new FormatException($"Date '{lastModifiedDate}' is not in the expected format (dd.MM.yyyy).");
            }

            LastModifiedDate = parsedDate;
        }
    }
}