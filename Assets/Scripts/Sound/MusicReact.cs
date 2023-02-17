using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MusicReact : MonoBehaviour
{
    public BeatDetection BD;
    // Start is called before the first frame update
    [SerializeField] protected VisualEffect vfx;
    [SerializeField] protected ParticleBoids boids;

    protected float defaultColorBrightness;
    protected float defaultSeparationFac;
    protected int vfxPropFlowBrightness = Shader.PropertyToID("ColorBrightness");
    [SerializeField]
    protected float additionalBrightness = 10;
    [SerializeField]
    protected float tPower = 2;
    [SerializeField]
    protected float duration = 0.35f;

    public bool listenEnergy;
    public bool listenKick;
    public bool listenHat;
    public bool listenSnare;

    void Start()
    {
        BD.CallBackFunction = BeatHandler;
        defaultColorBrightness = vfx.GetFloat("ColorBrightness");
        defaultSeparationFac = boids.separationFac;
        
    }
    private void OnDisable()
    {
        //FlexibleMusicManager.Inst.gameObject.GetComponent<BeatDetection>().CallBackFunction = null;
    }
    void Blink()
    {        
        StartCoroutine(Coroutines.AnimateFrames(duration, (t) => {
            t = Mathf.Pow(t, tPower);
            vfx.SetFloat(vfxPropFlowBrightness, defaultColorBrightness + additionalBrightness * t);
        }, true, true));
        StartCoroutine(Coroutines.AnimateFrames(0.33f, (t) => {
            var t15 = Mathf.Pow(t, 1.5f);
            var t4 = Mathf.Pow(t, 4f);
            
            boids.speedFac = 1.8f + t15 * 3.5f;
            boids.separationFac = defaultSeparationFac + t4 * 1.15f;
            //vfx.SetFloat(vfxPropFlowBrightness, defaultColorBrightness + additionalBrightness * t);
        }, true, true));
    }
    public void BeatHandler(BeatDetection.EventInfo eventInfo)
    {
        
        switch (eventInfo.messageInfo)
        {
            case BeatDetection.EventType.Energy:
                if (listenEnergy) Blink();                
                break;
            case BeatDetection.EventType.HitHat:
                if (listenHat) Blink();
                //LightningOn();
                break;
            case BeatDetection.EventType.Kick:
                if (listenKick) Blink();
                //Blink();
                //VLightOn();
                break;
            case BeatDetection.EventType.Snare:
                if (listenSnare) Blink();
                //Blink();
                //BlueflareOn();
                break;
        }
    }

    
}
