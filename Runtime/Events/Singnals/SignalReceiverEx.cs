using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace UnityEngine.Timeline.Extensions
{
    /// <summary>
    /// Listens for emitted signals and reacts depending on its defined reactions.
    /// </summary>
    /// A SignalReceiver contains a list of reactions. Each reaction is bound to a SignalAsset.
    /// When a SignalEmitter emits a signal, the SignalReceiver invokes the corresponding reaction.
    /// <seealso cref="UnityEngine.Timeline.SignalEmitter"/>
    /// <seealso cref="UnityEngine.Timeline.SignalAsset"/>
    [DisallowMultipleComponent]
    public class SignalReceiverEx : MonoBehaviour, INotificationReceiver
    {
        [SerializeField]
        EventKeyValue m_Events = new EventKeyValue();
        [SerializeField]
        EventKeyValue m_PauseEvents = new EventKeyValue();

        /// <summary>
        /// Called when a notification is sent.
        /// </summary>
        /// <param name="origin">The playable that sent the notification.</param>
        /// <param name="notification">The received notification. Only notifications of type <see cref="SignalEmitter"/> will be processed.</param>
        /// <param name="context">User defined data that depends on the type of notification. Uses this to pass necessary information that can change with each invocation.</param>
        public void OnNotify(Playable origin, INotification notification, object context)
        {
            if (notification is not ISignalEmitterExProvider signal) return;
            if (!signal.isPause)
            {
                if (TryGetReaction(signal.notificationKey, out var evt))
                    evt?.Invoke(signal.notificationValue);
            }
            else
            {
                if (TryGetPauseReaction(signal.notificationKey, out var evt))
                    evt?.Invoke(signal.notificationValue);
            }
        }

        private void OnDestroy()
        {
            m_Events.Clear();
            m_PauseEvents.Clear();
        }

        /// <summary>
        /// Defines a new reaction for a SignalAsset.
        /// </summary>
        /// <param name="asset">The SignalAsset for which the reaction is being defined.</param>
        /// <param name="reaction">The UnityEvent that describes the reaction.</param>
        /// <exception cref="ArgumentNullException">Thrown when the asset is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the SignalAsset is already registered with this receiver.</exception>
        public void AddReaction(string asset, UnityEvent<string> reaction)
        {
            if (asset == null)
                throw new ArgumentNullException("asset");

            if (m_Events.signals.Contains(asset))
                throw new ArgumentException("SignalAsset already used.");
            m_Events.Append(asset, reaction);
        }

        public void AddAction(string key, UnityAction<string> action)
        {
            if (key == null)
            {
                Debug.LogError("AddAction key is null!");
                return;
            }
            if (TryGetReaction(key, out var reaction))
            {
                if (reaction == null)
                {
                    Debug.LogError($"AddAction reaction is null! key: {key}");
                    return;
                }
                //先移除再添加
                reaction.RemoveListener(action);
                reaction.AddListener(action);
            }
            else
            {
                reaction = new UnityEvent<string>();
                reaction.AddListener(action);
                AddReaction(key, reaction);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a SignalAsset.
        /// </summary>
        /// <param name="asset">The SignalAsset to be removed.</param>
        public void RemoveReaction(string asset)
        {
            if (!m_Events.signals.Contains(asset))
            {
                throw new ArgumentException("The SignalAsset is not registered with this receiver.");
            }

            m_Events.Remove(asset);
        }
        
        public void RemoveAction(string key, UnityAction<string> action)
        {
            if (key == null)
            {
                Debug.LogError("AddAction key is null!");
                return;
            }
            if (!TryGetReaction(key, out var reaction)) return;
            
            if (reaction == null)
            {
                Debug.LogError($"AddAction reaction is null! key: {key}");
                return;
            }
            reaction.RemoveListener(action);
        }

        /// <summary>
        /// Gets the first UnityEvent associated with a SignalAsset.
        /// </summary>
        /// <param name="key">A SignalAsset defining the signal.</param>
        /// <returns>Returns the reaction associated with a SignalAsset. Returns null if the signal asset does not exist.</returns>
        public UnityEvent<string> GetReaction(string key)
        {
            UnityEvent<string> ret;
            if (m_Events.TryGetValue(key, out ret))
            {
                return ret;
            }

            return null;
        }
        
        public bool TryGetReaction(string key, out UnityEvent<string> reaction)
        {
            UnityEvent<string> ret;
            if (m_Events.TryGetValue(key, out reaction))
            {
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Defines a new reaction for a SignalAsset.
        /// </summary>
        /// <param name="asset">The SignalAsset for which the reaction is being defined.</param>
        /// <param name="reaction">The UnityEvent that describes the reaction.</param>
        /// <exception cref="ArgumentNullException">Thrown when the asset is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the SignalAsset is already registered with this receiver.</exception>
        public void AddPauseReaction(string asset, UnityEvent<string> reaction)
        {
            if (asset == null)
                throw new ArgumentNullException("asset");

            if (m_PauseEvents.signals.Contains(asset))
                throw new ArgumentException("SignalAsset already used.");
            m_PauseEvents.Append(asset, reaction);
        }

        public void AddPauseAction(string key, UnityAction<string> action)
        {
            if (key == null)
            {
                Debug.LogError("AddAction key is null!");
                return;
            }
            if (TryGetPauseReaction(key, out var reaction))
            {
                if (reaction == null)
                {
                    Debug.LogError($"AddAction reaction is null! key: {key}");
                    return;
                }
                //先移除再添加
                reaction.RemoveListener(action);
                reaction.AddListener(action);
            }
            else
            {
                reaction = new UnityEvent<string>();
                reaction.AddListener(action);
                AddPauseReaction(key, reaction);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a SignalAsset.
        /// </summary>
        /// <param name="asset">The SignalAsset to be removed.</param>
        public void RemovePauseReaction(string asset)
        {
            if (!m_PauseEvents.signals.Contains(asset))
            {
                throw new ArgumentException("The SignalAsset is not registered with this receiver.");
            }

            m_PauseEvents.Remove(asset);
        }
        
        public void RemovePauseAction(string key, UnityAction<string> action)
        {
            if (key == null)
            {
                Debug.LogError("AddAction key is null!");
                return;
            }
            if (!TryGetPauseReaction(key, out var reaction)) return;
            
            if (reaction == null)
            {
                Debug.LogError($"AddAction reaction is null! key: {key}");
                return;
            }
            reaction.RemoveListener(action);
        }

        /// <summary>
        /// Gets the first UnityEvent associated with a SignalAsset.
        /// </summary>
        /// <param name="key">A SignalAsset defining the signal.</param>
        /// <returns>Returns the reaction associated with a SignalAsset. Returns null if the signal asset does not exist.</returns>
        public UnityEvent<string> GetPauseReaction(string key)
        {
            UnityEvent<string> ret;
            if (m_PauseEvents.TryGetValue(key, out ret))
            {
                return ret;
            }

            return null;
        }
        
        public bool TryGetPauseReaction(string key, out UnityEvent<string> reaction)
        {
            UnityEvent<string> ret;
            if (m_PauseEvents.TryGetValue(key, out reaction))
            {
                return true;
            }

            return false;
        }

        [Serializable]
        public class EventKeyValue
        {
            [SerializeField]
            List<string> m_Signals = new List<string>();

            [SerializeField]
            List<UnityEvent<string>> m_Events = new List<UnityEvent<string>>();

            public bool TryGetValue(string key, out UnityEvent<string> value)
            {
                var index = m_Signals.IndexOf(key);
                if (index != -1)
                {
                    value = m_Events[index];
                    return true;
                }

                value = null;
                return false;
            }

            public void Append(string key, UnityEvent<string> value)
            {
                m_Signals.Add(key);
                m_Events.Add(value);
            }

            public void Remove(int idx)
            {
                if (idx != -1)
                {
                    m_Signals.RemoveAt(idx);
                    m_Events.RemoveAt(idx);
                }
            }

            public void Remove(string key)
            {
                var idx = m_Signals.IndexOf(key);
                if (idx != -1)
                {
                    m_Signals.RemoveAt(idx);
                    m_Events.RemoveAt(idx);
                }
            }

            public void Clear()
            {
                m_Signals.Clear();
                m_Events.Clear();
            }

            public List<string> signals
            {
                get { return m_Signals; }
            }

            public List<UnityEvent<string>> events
            {
                get { return m_Events; }
            }
        }
    }
}
