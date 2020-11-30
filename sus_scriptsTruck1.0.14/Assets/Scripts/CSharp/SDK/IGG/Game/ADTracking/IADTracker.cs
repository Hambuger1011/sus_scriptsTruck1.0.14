using System.Collections.Generic;

namespace ADTracking
{
    public interface IADTracker
    {
        void Init(string customerInfo);
        void Track(string name, Dictionary<string, object> extraInfos);
    }
}