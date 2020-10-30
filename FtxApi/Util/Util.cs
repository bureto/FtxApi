using System;
using System.IO;

namespace FtxApi.Util
{
    public static class Util
    {
        private static DateTime _epochTime = new DateTime(1970, 1, 1, 0, 0, 0);

        public static long GetMillisecondsFromEpochStart()
        {
            return GetMillisecondsFromEpochStart(DateTime.UtcNow);
        }

        public static long GetMillisecondsFromEpochStart(DateTime time)
        {
            return (long)(time - _epochTime).TotalMilliseconds;
        }

        public static long GetSecondsFromEpochStart(DateTime time)
        {
            return (long)(time - _epochTime).TotalSeconds;
        }

        public static bool IsPerpetual(string name)
        {
            return name.Contains("-PERP");
        }
        public static string GetUnderlyingName(string name)
        {
            var slashIndex = name.IndexOf("/");
            if (slashIndex != -1)
                return name.Substring(0, slashIndex);
            else
            {
                var dashIndex = name.IndexOf("-");
                if (dashIndex != -1)
                    return name.Substring(0, dashIndex);
                else
                    throw new ArgumentException("Market instrument name did not contain a dash (-) or a slash (/).");
            }
        }
    }

}
