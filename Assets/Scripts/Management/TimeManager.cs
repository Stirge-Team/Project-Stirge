using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    #region Singleton
    private static TimeManager s_instance;
    public static TimeManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                FindFirstObjectByType<TimeManager>().Init();
            }
            return s_instance;
        }
    }
    private void Init()
    {
        s_instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    [SerializeField, Min(0)] private float m_defaultTimeScale = 1;

    private Coroutine m_currentTimeScaleCoroutine;

    #region Unity Events
    private void Awake()
    {
        Init();
        SetDefaultTimeScale();
    }
    #endregion

    public void SetTimeScale(float timeScale)
    {
        if (timeScale < 0)
        {
            Debug.LogError("Cannot provide negative TimeScale value!", this);
            return;
        }

        StopTimeScaleCoroutine();

        Time.timeScale = timeScale;
    }
    public void SetDefaultTimeScale()
    {
        StopTimeScaleCoroutine();

        Time.timeScale = m_defaultTimeScale;
    }

    public void SetTimeScaleForTime(float timeScale, float time)
    {
        if (timeScale < 0)
        {
            Debug.LogError("Cannot provide negative TimeScale value!", this);
            return;
        }

        StopTimeScaleCoroutine();

        m_currentTimeScaleCoroutine = StartCoroutine(TimeScaleForLength(timeScale, time));
    }

    private IEnumerator TimeScaleForLength(float timeScale, float time)
    {
        SetTimeScale(timeScale);
        yield return new WaitForSecondsRealtime(time);
        SetDefaultTimeScale();

        m_currentTimeScaleCoroutine = null;
    }

    private void StopTimeScaleCoroutine()
    {
        if (m_currentTimeScaleCoroutine != null)
            StopCoroutine(m_currentTimeScaleCoroutine);
    }
}
