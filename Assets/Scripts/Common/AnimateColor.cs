using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AnimateColor : MonoBehaviour
{
    [System.Serializable]
    public class State
    {
        public Color Color;
        public float WaitTimeBeforeFade = 0;
        public float FadeDuration = 1;
        public UnityEvent OnEnd;
    }
    public Image TargetImage;
    public List<State> States;

    private State s;
    private Color c
    {
        get { return TargetImage.color; }
        set {            
            TargetImage.color = value; }
        }
    public bool Paused;
    public bool Loop = false;
    private bool skip = false;
    public void Skip()
    {
        skip = true;
    }
    IEnumerator Animate()
    {
        int stateIndex = 0;
        
        
        while(stateIndex < States.Count || Loop)
        {
            while (Paused) yield return new WaitForEndOfFrame();

            if (stateIndex == States.Count) stateIndex = 0;
            s = States[stateIndex];
            yield return new WaitForSeconds(s.WaitTimeBeforeFade);
            var startColor = c;

            float t = 0;
            float i = 0;
            skip = false;
            while (t < s.FadeDuration)
            {
                i = t / s.FadeDuration;
                c = Color.Lerp(startColor, s.Color, i);
                if (skip)
                {                    
                    t = s.FadeDuration;                    
                }
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
            if (!skip) s.OnEnd.Invoke();
            stateIndex++;
        }
        

        yield return null;
    }

    public void SetPause(bool paused)
    {
        Paused = paused;
    }

    private void Awake()
    {
        StartCoroutine(Animate());
    }
}
