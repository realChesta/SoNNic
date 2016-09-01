using System;
using System.Text;
using System.Windows.Forms;

namespace RemotePanel
{
    public static class Extensions
    {
        #region ControlExtensions

        public static TResult InvokeEx<TControl, TResult>(this TControl control, Func<TControl, TResult> func)
          where TControl : Control
        {
            return control.InvokeRequired
                    ? (TResult)control.Invoke(func, control)
                    : func(control);
        }

        public static void InvokeEx<TControl>(this TControl control, Action<TControl> func)
          where TControl : Control
        {
            control.InvokeEx(c => { func(c); return c; });
        }

        public static void InvokeEx<TControl>(this TControl control, Action action)
          where TControl : Control
        {
            control.InvokeEx(c => action());
        }

        #endregion

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
            timeBuilder.Append(time.Seconds.ToString() + "s ");

            return timeBuilder.ToString().Trim();
        }
    }
}
