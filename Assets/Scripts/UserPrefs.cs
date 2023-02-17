using UnityEngine;
[DefaultExecutionOrder(-60)]
public class UserPrefs : Singleton<UserPrefs>
{

    public const string LANGUAGE_KEY = "Language";

    [System.Serializable]
    public class SettingsData
    {
        public bool Fullscreen;
        public string Resolution;
        public SystemLanguage Language;
        public bool AltFiringMode;
        public bool PostProcessing;
        public float SoundVolume;
        public float MusicVolume;
        public bool Censorship;
    }

    public SettingsData Settings;


    public void ApplyNow()
    {
        
        ApplyResolution();
        I18n.Inst.Init();
        I18n.Inst.UpdateAllTextBoxes();
        //GraphicsSetup.Inst.SetPostprocessing(Settings.PostProcessing);

    }

    public void ApplyResolution()
    {
        if (Screen.currentResolution.ToString() != Settings.Resolution || Screen.fullScreen != Settings.Fullscreen)
        {
            foreach (var r in Screen.resolutions)
            {
                Debug.Log(r.ToString());
                if (r.ToString() == Settings.Resolution)
                {
                    Screen.SetResolution(r.width, r.height, Settings.Fullscreen);
                    return;
                }
            }
            Debug.LogError("Can't set resolution " + Settings.Resolution);
        } else
        {
            Debug.Log("Resolution change not needed");
        }
    }

    protected override void Awake()
    {
        base.Awake();        
        TryInit();
    }

    private void OnEnable()
    {
        TryInit();
    }

    private bool _initialized = false;
    void TryInit()
    {
        //PlayerPrefs.SetInt("Language", (int)SystemLanguage.Russian);
        if (_initialized) return;
        ReadData();
        //DontDestroyOnLoad(this.gameObject);
        _initialized = true;
    }

    private void Start()
    {
        ApplyNow();
    }


    public bool IsFirstRun => PlayerPrefs.GetInt(LANGUAGE_KEY, -1) == -1;

    public void ReadData()
    {
        Settings = new SettingsData();
        Settings.Fullscreen = (PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen?1:0) == 1);
        Settings.Resolution = PlayerPrefs.GetString("Resolution", Screen.currentResolution.ToString());
        Settings.Language = (SystemLanguage)PlayerPrefs.GetInt(LANGUAGE_KEY, (int)SystemLanguage.English);        
        Settings.PostProcessing = (PlayerPrefs.GetInt("PostProcessing", 1) == 1);
        Settings.SoundVolume = PlayerPrefs.GetFloat("SoundVolume", 0.5f);
        Settings.MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);        
    }

    public void WriteData()
    {        
        //PlayerPrefs.SetString("Resolution",Settings.Resolution);        
        //PlayerPrefs.SetInt("Fullscreen", Settings.Fullscreen ? 1 : 0);
        PlayerPrefs.SetInt(LANGUAGE_KEY, (int)Settings.Language);
        
        PlayerPrefs.SetInt("PostProcessing", Settings.PostProcessing ? 1 : 0);
        PlayerPrefs.SetFloat("SoundVolume", Settings.SoundVolume);
        PlayerPrefs.SetFloat("MusicVolume", Settings.MusicVolume);
        
        PlayerPrefs.Save();        
    }
}
