using UnityEngine;

public class HitStopManager : MonoBehaviour
{
    #region Singleton
    private static HitStopManager s_instance;
    public static HitStopManager Instance
    {
        get
        {
            if (s_instance == null)
                FindFirstObjectByType<HitStopManager>().Init();
            return s_instance;
        }
    }

    private void Init()
    {
        s_instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    [Header("HitStop Scaling Properties")]
    [Tooltip("Affects how the time length scales based on the input.\nMust be greater than 1 ")]
    [SerializeField, Min(1.001f)] private float m_scaling = 1.1f;
    [Tooltip("The maximum length of any HitStop.\nMust be greater than 0.")]
    [SerializeField, Min(0.001f)] private float m_maxValue = 1f;

    #region UnityEvents
    private void Awake()
    {
        Init();
    }
    #endregion

    public void HitStopTime(float damage)
    {
        //             m
        // func l =  ----- + m
        //            s^d
        // where l is the total timelength of the HitStop in seconds
        //       s is the scaling, must be greater than 1
        //       m is the max value, must be greater than 0
        //       d is the input damage, must be greater than 0
        float length = -(m_maxValue / Mathf.Pow(m_scaling, damage)) + m_maxValue;

        TimeManager.Instance.SetTimeScaleForTime(0f, length);
    }
}
