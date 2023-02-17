using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Serialization;

[SingletonSetup(InstanceName = "SFXManager", PrefabName = "SFXManager")]
public class SFXManager : Singleton<SFXManager>
{
    public const string VOICE = "Voices";
    public const string DEFAULT = "Default";

    [System.Serializable]
    public class Entity : SoundItem
    {
    }

    [HideInInspector]
    public Dictionary<string, AudioSource> SharedAudioSources;
    public class SharedAudioSrc
    {
        public string Name;
        public AudioSource Src;
    }

    public List<Entity> Collection;    
    public DropdownList<string> SoundsForDropdown
    {
        get
        {
            var l = new DropdownList<string>();
            l.Add("[NONE]", null);
            Collection.ForEach((i) => { l.Add(i.Name, i.Name); });
            return l;
        }

    }
    

    [FormerlySerializedAs("Mixer")]
    public AudioMixerGroup DefaultMixer;

    

    protected override void Awake()
    {
        base.Awake();        

        InitSharedAudioSources();
    }

    private AudioSource defaultAudioSrc;
    public AudioSource DefaultAudioSource => defaultAudioSrc;
    private void InitSharedAudioSources()
    {
        SharedAudioSources = new Dictionary<string, AudioSource>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var o = transform.GetChild(i).gameObject;
            var src = o.GetComponent<AudioSource>();
            var name = o.name;
            SharedAudioSources.Add(name, src);
        }
        defaultAudioSrc = SharedAudioSources["Default"];
    }
    public DropdownList<string> SharedAudioSourceNames
    {
        get
        {
            var l = new DropdownList<string>();
            l.Add("[NONE]", null);
            InitSharedAudioSources();
            foreach (var s in SharedAudioSources) { l.Add(s.Key, s.Key); }
            return l;
        }

    }


    public AudioSource CreateAudioSource(AudioMixerGroup mixer = null)
    {
        var a = gameObject.AddComponent<AudioSource>();
        if (mixer == null) mixer = DefaultMixer;
        a.outputAudioMixerGroup = mixer;
        return a;
    }

    public void Play(string name)
    {
        if (name == null || name == "") return;
        var e = Collection.Find(i => i.Name == name);
        if (e == null)
        {
            Debug.LogError($"SFXManager can't find Sound Entity named {name}");
            return;
        }
        e.Play();
    }

    public Coroutine StartExtCoroutine(IEnumerator Coroutine)
    {
        return StartCoroutine(Coroutine);
    }

    private IEnumerator Play( AudioClip clip, AudioSource audioSrc,  float delay, System.Action onEnd = null)
    {
        if (clip == null) yield break;
        yield return new WaitForSeconds(delay);
        audioSrc.PlayOneShot(clip);
        if (onEnd != null)
        {
            yield return new WaitForSeconds(clip.length);
            onEnd.Invoke();
        }

    }

    public Coroutine Play(AudioClip clip, string audioSrc = DEFAULT, float delay = 0f, System.Action onEnd = null)
    {        
        return StartCoroutine(Play(clip, SharedAudioSources[audioSrc], delay, onEnd));
    }
}
