using System.Diagnostics;

namespace SlackUtility
{
    public class TraceLog
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