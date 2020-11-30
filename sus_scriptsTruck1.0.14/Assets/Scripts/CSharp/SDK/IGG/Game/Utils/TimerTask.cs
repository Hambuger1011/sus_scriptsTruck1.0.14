using IGG.SDK.Foundation.Thread;

namespace IGGUtils
{
    internal class TimerTask
    {
        public float nextExecutionTime;
        public bool isScheduled = false;
        public Runnable runnable;
    }
}