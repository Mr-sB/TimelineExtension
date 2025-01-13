using UnityEngine.Playables;

namespace UnityEngine.Timeline.Extensions
{
    public interface ISignalEmitterExProvider : INotification, INotificationOptionProvider
    {
        public string notificationKey { get; }
        
        public string notificationValue { get; }
        
        public bool isPause { get; }
    }
}