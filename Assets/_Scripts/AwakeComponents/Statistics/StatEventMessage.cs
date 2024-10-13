using System;

namespace AwakeComponents.Statistics
{
    [Serializable]
    public class StatEventMessage
    {
        public string appName;
        public string appVersion;
        public string statsVersion;
        public string eventTime;
        public string eventName;
        public string platform;

        public StatEventMessage(
            string appName,
            string appVersion,
            string eventTime,
            string eventName,
            string statsVersion,
            string platform = "Editor"
        )
        {
            this.appName = appName;
            this.appVersion = appVersion;
            this.statsVersion = statsVersion;
            this.eventTime = eventTime;
            this.eventName = eventName;
            this.platform = platform;
        }
    }
}