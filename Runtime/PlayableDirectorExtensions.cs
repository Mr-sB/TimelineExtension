using System.Reflection;
using UnityEngine.Playables;

namespace UnityEngine.Timeline.Extensions
{
    public static class PlayableDirectorExtensions
    {
        public static TrackAsset FindTrackAsset(this PlayableDirector playableDirector, string trackName)
        {
            if (!playableDirector) return null;
            foreach (TrackAsset trackAsset in ((TimelineAsset)playableDirector.playableAsset).GetOutputTracks())
            {
                if (trackAsset.name == trackName)
                {
                    return trackAsset;
                }
            }

            return null;
        }

        public static Object GetBingingObject(this PlayableDirector playableDirector, string trackName)
        {
            var trackAsset = FindTrackAsset(playableDirector, trackName);
            if (!trackAsset) return null;
            return playableDirector.GetGenericBinding(trackAsset);
        }

        public static void SetBingingObject(this PlayableDirector playableDirector, string trackName, Object bindingObject)
        {
            var trackAsset = FindTrackAsset(playableDirector, trackName);
            if (!trackAsset) return;
            playableDirector.SetGenericBinding(trackAsset, bindingObject);
        }

        public static void SetBingingObjectAndCheck(this PlayableDirector playableDirector, string trackName, GameObject bindingGO)
        {
            var trackAsset = FindTrackAsset(playableDirector, trackName);
            if (!trackAsset) return;
            if (!(trackAsset.GetType().GetCustomAttribute(typeof(TrackBindingTypeAttribute)) is TrackBindingTypeAttribute attribute))
                return;
            var bindingType = attribute.type;
            Object bindingObject;

            if (bindingType == typeof(GameObject))
                bindingObject = bindingGO;
            else if (bindingType == typeof(Transform))
                bindingObject = bindingGO.transform;
            else
            {
                bindingObject = bindingGO.GetComponent(bindingType);
                if (!bindingObject)
                    bindingObject = bindingGO.AddComponent(bindingType);
            }

            playableDirector.SetGenericBinding(trackAsset, bindingObject);
        }
    }
}
