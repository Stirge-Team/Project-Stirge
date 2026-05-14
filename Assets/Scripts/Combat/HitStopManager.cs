using System.Collections;
using UnityEngine;

public class HitStopManager : MonoBehaviour
{
    #region Singleton
    public static HitStopManager Instance { get; private set; }
    private void AllocateInstance()
    {
        // prioritise existing Instance
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }
    #endregion

    private Coroutine m_currentHitstop;

    #region UnityEvents
    private void Awake()
    {
        AllocateInstance();
    }
    #endregion

    public void Stop(float seconds)
    {
        if (m_currentHitstop != null)
            StopCoroutine(m_currentHitstop);

        m_currentHitstop = StartCoroutine(HitStop(seconds));
    }

    private IEnumerator HitStop(float seconds)
    {
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(seconds);

        Time.timeScale = 1f;

        m_currentHitstop = null;
    }
}
