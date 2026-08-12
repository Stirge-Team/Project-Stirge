using System;
using UnityEngine;

[Serializable]
public class HitboxData
{
    [SerializeField] private Vector3 m_position;
    [SerializeField] private Vector3 m_scale;

    [SerializeField] private int m_startFrame;
    [SerializeField] private int m_endFrame;
}
