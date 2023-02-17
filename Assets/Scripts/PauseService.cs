using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PauseService : Singleton<PauseService>
{
    public bool TriggerByEsc = false;
    public bool IsPaused = false;
    public bool Locked = false;
    public GameObject PausePanel;
    public bool ShowHideCursor = false;

    [HideInInspector]
    public float TimeScale = 1f;

    Coroutine switching;

    PlayerControls controls;

    private void OnEnable()
    {
        controls?.GamePlay.Enable(); // ? -> was editor errors on pause
    }
    protected override void Awake()
    {
        base.Awake();
        controls = new PlayerControls();
        controls.GamePlay.TogglePause.performed += ctx => TriggerPause();
    }
    void Start()
    {
        if (PausePanel) PausePanel.SetActive(false);
        if (ShowHideCursor) Cursor.visible = IsPaused;
    }

    void OnDisable()
    {
        StopAllCoroutines();
        Time.timeScale = 1;
        controls?.GamePlay.Disable();
    }

    void Update()
    {
        if (!TriggerByEsc) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TriggerPause();
        }

    }

    public void Pause(bool nosound = false)
    {
        SetPause(true, nosound);
    }

    public void TriggerPause()
    {
        SetPause(!IsPaused);
    }

    public void SetPause(bool pause, bool nosound = false)
    {
        
        if (Locked) return;
        if (IsPaused == pause) return;
        IsPaused = pause;

        if (nosound != true && pause == true)
        {
            var lang = (I18n.Inst.CurrentLanguage == "English") ? "EN" : "UA";
            SFXManager.Inst.Play($"Voice.Pause.{lang}");
        }

        if (ShowHideCursor) Cursor.visible = IsPaused;
        if (!Application.isEditor) Cursor.visible = pause;
        //Time.timeScale = pause?0:1;
        //PausePanel.SetActive(pause);
        if (switching != null) StopCoroutine(switching);
        switching = StartCoroutine(pause ? Pausing() : Unpausing());
    }
    public void Unpause()
    {
        SetPause(false);
    }

    private IEnumerator Pausing()
    {
        float step;
        step = 1f / 20f;

        PausePanel.SetActive(true);
        var cg = PausePanel.GetComponent<CanvasGroup>();
        cg.alpha = 0;


        Time.timeScale = TimeScale;
        float i = 1;
        while (i > 0)
        {
            i -= step;
            Time.timeScale = Mathf.Clamp(i, 0, 1);
            yield return new WaitForEndOfFrame();
        }
        Time.timeScale = 0.001f;

        step = 1f / 30f;
        i = 0;
        while (i < 1)
        {
            i += step;
            cg.alpha = Mathf.Clamp(i, 0, 1);
            yield return new WaitForEndOfFrame();
        }
        cg.alpha = 1;
        Time.timeScale = 0;
        switching = null;
        yield return null;
    }

    private IEnumerator Unpausing()
    {
        float step = 1f / 30f;

        var cg = PausePanel.GetComponent<CanvasGroup>();
        cg.alpha = 1;

        float i = 1;
        while (i > 0)
        {
            i -= step * 4;
            cg.alpha = Mathf.Clamp(i, 0, 1);
            yield return new WaitForEndOfFrame();
        }
        cg.alpha = 0;
        PausePanel.SetActive(false);

        step = 1f / 80f;
        Time.timeScale = 0;
        i = 0;
        while (i < 1)
        {
            i += step;
            Time.timeScale = Mathf.Clamp(i, 0, TimeScale);
            yield return new WaitForEndOfFrame();
        }
        Time.timeScale = TimeScale;
        switching = null;
        yield break;
    }
}
