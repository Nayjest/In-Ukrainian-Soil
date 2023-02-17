using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IEnumeratorExtentions
{
    public static IEnumerator Then(this IEnumerator i, IEnumerator second)
    {
        yield return i;
        yield return second;
    }

    public static IEnumerator ThenWait(this IEnumerator i, float seconds)
    {
        yield return i;
        yield return new WaitForSeconds(seconds);
    }
}

public class Coroutines
{
    public static IEnumerator MoveToBySpeed(Transform o, Vector3 pos, float speed)
    {

        Vector3 diff;
        do
        {
            o.position += Vector3.one * Time.deltaTime;
            diff = o.position - pos;
            var frameDist = speed * Time.deltaTime;
            if (frameDist > diff.magnitude) frameDist = diff.magnitude;
            o.position += diff.normalized * frameDist;
            yield return new WaitForEndOfFrame();
        } while (diff.magnitude > 0.1f);
        yield break;
    }

    public static IEnumerator RunWithDelay(float delay, System.Action then)
    {
        yield return new WaitForSeconds(delay);
        then.Invoke();
    }

    public static IEnumerator RunOnNextFrame(System.Action then)
    {
        yield return new WaitForEndOfFrame();
        then.Invoke();
    }

    public static IEnumerator TimeSlow(float timeScale, float durationIn, float durationOut, float durationWait = 0)
    {
        float t = 0;
        var prevTimeScale = Time.timeScale;
        while (t <= durationIn)
        {
            Time.timeScale = Mathf.Lerp(prevTimeScale, timeScale, t / durationIn);
            t += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        Time.timeScale = timeScale;
        yield return new WaitForSecondsRealtime(durationWait);

        t = 0;
        prevTimeScale = Time.timeScale;
        while (t <= durationOut)
        {

            Time.timeScale = Mathf.Lerp(prevTimeScale, 1.0f, t / durationOut);
            t += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        Time.timeScale = 1.0f;
    }

    public static IEnumerator Scale(Transform target, float multiplier, float duration, bool relative = false)
    {
        return Scale(target, multiplier * Vector3.one, duration, relative);
    }
    public static IEnumerator Scale(Transform target, Vector3 multiplier, float duration, bool relative = false, bool scaledTime = false)
    {
        var initScale = target.localScale;
        float t = 0;
        while (t <= duration)
        {
            var step = t / duration;
            if (relative)
            {
                target.localScale = new Vector3(
                    Mathf.SmoothStep(1, multiplier.x, step) * initScale.x,
                    Mathf.SmoothStep(1, multiplier.y, step) * initScale.y,
                    Mathf.SmoothStep(1, multiplier.z, step) * initScale.z
                );
            }
            else
            {
                target.localScale = new Vector3(
                    Mathf.SmoothStep(initScale.x, multiplier.x, step),
                    Mathf.SmoothStep(initScale.y, multiplier.y, step),
                    Mathf.SmoothStep(initScale.z, multiplier.z, step)
                );

            }

            t += scaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        target.localScale = relative ? Vector3.Scale(initScale, multiplier) : multiplier;
        yield break;
    }

    public static IEnumerator AnimateFrames(float duration, System.Action<float> AnimationStep, bool reverse = false, bool unscaled = false)
    {
        //yield return new WaitForEndOfFrame();
        float t = 0;
        float i = 0;
        while (t < duration)
        {
            i = t / duration;
            AnimationStep(reverse ? (1.0f - i) : i);
            yield return new WaitForEndOfFrame();
            t += (unscaled? Time.unscaledDeltaTime : Time.deltaTime);
        }
        AnimationStep(reverse ? 0 : 1);
        yield break;
    }
}
