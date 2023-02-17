using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public  class SoundItem: ISerializationCallbackReceiver
{
    [AllowNesting]
    [ShowIf("ShowName")]
    public string Name;   

    [AllowNesting]    
    public ObjectSource Src;

    [AllowNesting]
    public float Cooldown = 0;

    [AllowNesting]
    [Range(0,0.8f)]
    public float RandomizePitch = 0.1f;

    [AllowNesting]
    [Range(-3, 3)]
    public float BasePitch = 1.0f;

    [AllowNesting]
    [Range(0, 1f)]
    public float PlayChance = 1.0f;

    public float StartDelay = 0;

    [AllowNesting]
    [Range(0, 1f)]
    public float Volume = 1.0f;

    public bool AddClipLengthToCoolDown = false;

    [AllowNesting]
    [Dropdown("SharedAudioSourceNames")]
    public string NamedAudioSource = null;

    private float lastClipLength = 0;

    public bool OnCooldown => Time.unscaledTime - lastTime <= (AddClipLengthToCoolDown ? lastClipLength + Cooldown : Cooldown);

    public DropdownList<string> SharedAudioSourceNames => SFXManager.Inst.SharedAudioSourceNames;

    public bool HasLocalization = false;

    [System.Serializable]
    public class LocalizedSound
    {        
        public SystemLanguage Language;
        public AudioClip Localized;
    }

    public List<LocalizedSound> LocalizedSounds;

    public AudioMixerGroup Mixer;

    protected float lastTime = 0;    

    protected virtual bool ShowName()
    {
        return true;
    }

    protected virtual AudioClip GetAudioClip()
    {
        if (HasLocalization && LocalizedSounds !=null)
        {
            var l = UserPrefs.Inst.Settings.Language;
            foreach (var i in LocalizedSounds)
            {
                if (i.Language == l) return i.Localized;
            }
            
        }
        var clip = Src.Choose();
        if (!(clip is AudioClip))
        {
            Debug.LogError($"Error getting audio clip from object source");
            return null;
        }
        return (AudioClip)clip;
    }

    protected AudioSource audioSrc;
    protected virtual AudioSource GetAudioSource()
    {
        if (audioSrc == null)
        {
            audioSrc = InitAudioSource();
        }
        return audioSrc;
    }

    protected virtual AudioSource InitAudioSource()
    {
        if (NamedAudioSource != null && NamedAudioSource != "") return SFXManager.Inst.SharedAudioSources[NamedAudioSource];
        return SFXManager.Inst.CreateAudioSource(Mixer);
    }


    private IEnumerator PlayWithDelay()
    {
        yield return new WaitForSecondsRealtime(StartDelay);
        PlayImmediate();
    }

    public void Play()
    {
        if (OnCooldown) return;
        if (PlayChance != 1.0f && Random.value > PlayChance) return;        
        if (StartDelay == 0)
        {         
            PlayImmediate();
        } else
        {            
            SFXManager.Inst.StartExtCoroutine(PlayWithDelay());
        }
    }

    public void PlayImmediate()
    {
        var clip = GetAudioClip();
        if (clip == null)
        {
            lastClipLength = 0;
            return;
        }
        lastClipLength = clip.length;
        lastTime = Time.unscaledTime;
        var src = GetAudioSource();
        if (Mixer && src.outputAudioMixerGroup == null) src.outputAudioMixerGroup = Mixer;
        src.pitch = BasePitch + (Random.value - 0.5f) * 2 * RandomizePitch;
        src.volume = Volume;
        src.PlayOneShot(clip);
    }

    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        lastTime = 0;
        lastClipLength = 0;
    }
}
