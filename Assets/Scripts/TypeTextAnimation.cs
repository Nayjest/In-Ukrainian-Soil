using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DefaultExecutionOrder(10000)]
public class TypeTextAnimation : MonoBehaviour
{
    private TMP_Text t;
    private string str;
    public AudioSource Audio;

    public Vector2 MinMaxSymbolAppearTime = new Vector2(0.01f, 0.04f);

    public float StartDelay = 0;
    public float Duration = 4;
    
    public bool AutoStart = true;
    public bool Wait = false;

    public bool AutoActivateNext = false;

    public bool AllowSkip = false;

    private bool skipped = false;
    
    [HideIf("AutoActivateNext")]
    public UnityEvent OnEnd;

    private void Awake()
    {
        t = GetComponent<TMP_Text>();
        t.enabled = false;
    }

    void Start()
    {
        
        if (Audio == null) Audio = GetComponent<AudioSource>();
        str = t.text;        
        if (AutoStart) Begin();
        
    }

    public void Continue()
    {
        Wait = false;
    }
    public void Begin()
    {
        StartCoroutine(Animate());
    }

    private void Update()
    {
        // https://www.reddit.com/r/Unity3D/comments/1syswe/ps4_controller_map_for_unity/
        // https://answers.unity.com/questions/411950/find-out-if-any-button-on-any-gamepad-has-been-pre.html
        if (AllowSkip && 
            Input.GetMouseButtonDown(1) ||
            Input.GetMouseButtonDown(0) ||
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetKeyDown(KeyCode.JoystickButton9))
        {
            skipped = true;
            if (SFXManager.Inst.SharedAudioSources.ContainsKey(SFXManager.VOICE))
                SFXManager.Inst.SharedAudioSources[SFXManager.VOICE].Stop();
        }
    }
    IEnumerator Animate()
    {
        
        yield return new WaitForSeconds(StartDelay);
        var startTime = Time.time;
        t.enabled = true;
        var s = "";
        t.text = "";
        for(int i = 0; i < str.Length; i++)
        {
            s += str[i];
            t.text = s;
            if (Audio && Random.value < 0.3f)
            {
                Audio.pitch = 0.5f + Random.value;
                Audio.PlayOneShot(Audio.clip);
            }
            yield return new WaitForSeconds(Random.Range(MinMaxSymbolAppearTime.x, MinMaxSymbolAppearTime.y));
            if (skipped)
            {
                t.text = str;
                break;
            }
        }

        var fadeOutDuration = 0.5f;
        var waitTime = Duration - (Time.time - startTime) - fadeOutDuration;
        
        float tmr = 0;
        while (tmr < waitTime)
        {
            yield return new WaitForEndOfFrame();
            tmr += Time.deltaTime;
            if (skipped) break;
        }
        //yield return new WaitForSeconds(waitTime);
        
        while (Wait)
        {
            yield return new WaitForEndOfFrame();
        }
        /*
        var tm = 0f;
        var startColor = t.color;
        var endColor = startColor;
        endColor.a = 0;
        while (tm<fadeOutDuration)
        {
            tm += Time.deltaTime;
            t.color = Color.Lerp(startColor, endColor, tm / fadeOutDuration);
            yield return new WaitForEndOfFrame();
        }
        */
        if (AutoActivateNext)
        {
            transform.parent.GetChild(transform.GetSiblingIndex() + 1).gameObject.SetActive(true);
        }
        else
        {
            if (OnEnd != null)
            {
                OnEnd.Invoke();
            }
        }
        //GameObject.Destroy(gameObject);
        yield return null;
    }

}
