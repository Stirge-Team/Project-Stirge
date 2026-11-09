using UnityEngine;
using UnityEngine.Playables;

namespace Stirge.AttackTimeline.StatusTrack
{
    using Combat;
    using Combat.Hitboxes;

    [System.Serializable]
    public class StatusBehaviour : PlayableBehaviour
    {
        [SerializeField] private StatusInflictData m_inflictData;

        private CombatEntity m_boundEntity;
        private bool m_inflicted = false;

        public static ScriptPlayable<StatusBehaviour> Create(PlayableGraph graph, StatusBehaviour template)
        {
            return ScriptPlayable<StatusBehaviour>.Create(graph, template);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            // If game is not running, do not try to apply status
            if (!Application.isPlaying)
                return;

            if (m_inflicted)
                return;

            if (m_boundEntity == null)
            {
                m_boundEntity = playerData as CombatEntity;
            }

            if (m_boundEntity == null)
                return;

            m_boundEntity.InflictStatus(m_boundEntity, m_boundEntity.CreateRuntimeStatusFromCurrentActionAtIndex(m_inflictData.StatusActionIndex), m_inflictData);

            m_inflicted = true;
        }
    }
}
