using System;

namespace UnityEngine.Timeline.Extensions
{
    /// <summary>
    /// Use this track to emit signals to a bound SignalReceiver.
    /// </summary>
    /// <remarks>
    /// This track cannot contain clips.
    /// </remarks>
    /// <seealso cref="UnityEngine.Timeline.SignalEmitter"/>
    /// <seealso cref="UnityEngine.Timeline.SignalReceiver"/>
    /// <seealso cref="UnityEngine.Timeline.SignalAsset"/>
    [Serializable]
    [TrackBindingType(typeof(SignalReceiverEx))]
    [TrackColor(0.25f, 0.25f, 0.25f)]
    [TrackClipType(typeof(SignalEmitterRange))]
    [ExcludeFromPreset]
    public class SignalTrackEx : MarkerTrack { }
}