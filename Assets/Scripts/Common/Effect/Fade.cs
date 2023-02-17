using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Fade : AbstractEffect
{
    public enum FadeMode
    {
        FadeIn,
        FadeOut
    }

    public FadeMode Mode = FadeMode.FadeOut;

    private Color finalColor;
    private Color startColor;
    public float Duration = 0.15f;
    private float timePassed = 0;
    private Material m;

    public override float GetDuration()
    {
        return Duration;
    }

    private void Start()
    {
        m = GetComponent<Renderer>().material;
        if (Mode == FadeMode.FadeOut)
        {
            startColor = m.color;
            finalColor = new Color(startColor.r, startColor.g, startColor.b, 0);
        }
        else
        {
            finalColor = m.color;
            m.color = startColor = new Color(startColor.r, startColor.g, startColor.b, 0);
        }

        timePassed = 0;
    }
    /*
    public override void Activate()
    {


        base.Activate();
    }
    */
    public void Update()
    {
        if(!Active)
        {
            return;
        }
        
        timePassed += Time.deltaTime;
        if (timePassed <= Duration) {
            SetColor(Color.Lerp(startColor, finalColor, 1-(Duration - timePassed)/Duration));
            return;
        }
        FinishEffect();
    }

    protected virtual void SetColor(Color color)
    {
        m.color = color;
    }
}
