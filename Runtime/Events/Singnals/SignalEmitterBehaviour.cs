using UnityEngine.Playables;

namespace UnityEngine.Timeline.Extensions
{
    public class SignalEmitterBehaviour : PlayableBehaviour
    {
        public SignalEmitterRange emitter;
        private INotificationReceiver m_Receiver;
        private bool m_IsEmitted;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            //只触发一次
            if (m_IsEmitted) return;
            m_IsEmitted = true;
            if (playerData is not INotificationReceiver receiver) return;
            m_Receiver = receiver;
            emitter.m_IsPause = false;
            receiver.OnNotify(playable, emitter, null);
        }
        
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            //重置触发标记
            m_IsEmitted = false;
        }
        
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            //重置触发标记
            m_IsEmitted = false;
            //暂停时触发
            emitter.m_IsPause = true;
            m_Receiver?.OnNotify(playable, emitter, null);
            m_Receiver = null;
        }
    }
}