using System.Diagnostics;

namespace SlackUtilityWeb
{
    public static class TraceLog
    {
        public static void Trace(TraceEventType type, string msg)
        {
            switch (type)
            {
                case TraceEventType.Verbose:
                    Debug.WriteLine(msg);
                    break;
                default:
                    System.Diagnostics.Trace.WriteLine(msg);
                    break;
            }
        }
    }
}