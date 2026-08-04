using System;
using UnityEngine;
using UnityEngine.Playables;

#if UNITY_EDITOR
using System.ComponentModel;
[DisplayName("Animation State Clip")]
#endif
[Serializable]
public class AnimationStatePlayableAsset : PlayableAsset
{
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        return Playable.Create(graph);
    }
}
