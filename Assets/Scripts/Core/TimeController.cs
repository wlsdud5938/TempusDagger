using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class TimeController : MonoBehaviour
{
    [Header("Defaults")]
    [Range(0.01f, 1f)] public float hitStopScale = 0.05f;
    public float defaultFixedDelta = 0.02f;

    private Coroutine slowRoutine;
    private Coroutine hitStopRoutine;

    private void Awake()
    {
        defaultFixedDelta = Time.fixedDeltaTime;
    }

    public void SlowFor(float duration, float scale)
    {
        if (slowRoutine != null) StopCoroutine(slowRoutine);
        slowRoutine = StartCoroutine(SlowRoutine(duration, Mathf.Clamp(scale, 0.01f, 1f)));
    }

    public void HitStop(float duration)
    {
        if (hitStopRoutine != null) StopCoroutine(hitStopRoutine);
        hitStopRoutine = StartCoroutine(HitStopRoutine(duration));
    }

    private IEnumerator SlowRoutine(float duration, float scale)
    {
        Time.timeScale = scale;
        Time.fixedDeltaTime = defaultFixedDelta * scale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDelta;
        slowRoutine = null;
    }

    private IEnumerator HitStopRoutine(float duration)
    {
        float prevScale = Time.timeScale;
        float prevFixed = Time.fixedDeltaTime;
        Time.timeScale = hitStopScale;
        Time.fixedDeltaTime = defaultFixedDelta * hitStopScale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = prevScale;
        Time.fixedDeltaTime = prevFixed;
        hitStopRoutine = null;
    }
}
