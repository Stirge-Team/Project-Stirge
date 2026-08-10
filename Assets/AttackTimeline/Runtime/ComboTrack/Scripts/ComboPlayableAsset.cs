using System;
using UnityEngine;
using UnityEngine.Playables;

#if UNITY_EDITOR
using System.ComponentModel;
[DisplayName("Combo Clip")]
#endif
[Serializable]
public class ComboPlayableAsset : PlayableAsset
{
    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        return Playable.Create(graph);
    }
}
