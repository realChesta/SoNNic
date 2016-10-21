using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analyzer
{
    public static class Utils
    {
        #region Extensions
        private static StringBuilder timeBuilder = new StringBuilder();
        public static string ToReadableString(this TimeSpan time)
        {
            timeBuilder.Clear();

            if (time.Days > 0)
            {
                timeBuilder.Append(time.Days.ToString() + "d ");
            }
            if (time.Hours > 0)
            {
                timeBuilder.Append(time.Hours.ToString() + "h ");
            }
            if (time.Minutes > 0)
            {
                timeBuilder.Append(time.Minutes.ToString() + "min ");
            }
            //else
            //{
            timeBuilder.Append(time.Seconds.ToString() + "s ");
            //}
            return timeBuilder.ToString().Trim();
        }
        #endregion
    }
}
