using System;
using UnityEngine;
using UnityEngine.Playables;

namespace UnityEngine.Timeline.Extensions
{
    /// <summary>
    /// Marker that emits a signal to a SignalReceiver.
    /// </summary>
    /// A SignalEmitter emits a notification through the playable system. A SignalEmitter is used with a SignalReceiver and a SignalAsset.
    /// <seealso cref="UnityEngine.Timeline.SignalAsset"/>
    /// <seealso cref="UnityEngine.Timeline.SignalReceiver"/>
    /// <seealso cref="UnityEngine.Timeline.Marker"/>
    [Serializable]
    [CustomStyle("SignalEmitter")]
    [ExcludeFromPreset]
    public class SignalEmitterRange : PlayableAsset, ISignalEmitterExProvider
    {
        [SerializeField] bool m_Retroactive;
        [SerializeField] bool m_EmitOnce;
        [SerializeField] private string m_NotificationKey;
        [SerializeField] private string m_NotificationValue;
        [NonSerialized] public bool m_IsPause;

        /// <summary>
        /// Use retroactive to emit the signal if playback starts after the SignalEmitter time.
        /// </summary>
        public bool retroactive
        {
            get { return m_Retroactive; }
            set { m_Retroactive = value; }
        }

        /// <summary>
        /// Use emitOnce to emit this signal once during loops.
        /// </summary>
        public bool emitOnce
        {
            get { return m_EmitOnce; }
            set { m_EmitOnce = value; }
        }

        public string notificationKey
        {
            get { return m_NotificationKey; }
            set { m_NotificationKey = value; }
        }
        
        public string notificationValue
        {
            get { return m_NotificationValue; }
            set { m_NotificationValue = value; }
        }

        public bool isPause
        {
            get { return m_IsPause; }
        }

        PropertyName INotification.id
        {
            get
            {
                if (m_NotificationKey != null)
                {
                    return new PropertyName(m_NotificationKey);
                }
                return new PropertyName(string.Empty);
            }
        }

        NotificationFlags INotificationOptionProvider.flags
        {
            get
            {
                return (retroactive ? NotificationFlags.Retroactive : default(NotificationFlags)) |
                    (emitOnce ? NotificationFlags.TriggerOnce : default(NotificationFlags)) |
                    NotificationFlags.TriggerInEditMode;
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<SignalEmitterBehaviour> playable = ScriptPlayable<SignalEmitterBehaviour>.Create(graph);
            playable.GetBehaviour().emitter = this;
            return playable;
        }
    }
}
