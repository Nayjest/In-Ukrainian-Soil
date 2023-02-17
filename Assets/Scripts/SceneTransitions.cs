using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitions : Singleton<SceneTransitions>
{
    const string SCENE_GAME = "Game";
    const string SCENE_MENU = "Menu";
    const string SCENE_CREDITS = "Credits";
    const string SCENE_HELP = "Help";
    const string SCENE_INTRO = "Intro";
    const string SCENE_OPENING = "Opening";

    public float FadeFromBlackDuration;
    public float FadeToBlackDuration;
    public float Delay = 0.1f;
    public float SceneLoadDelay = 0;

    public float TimePower = 1;

    public void GoToMenu()
    {
        GoTo(SCENE_MENU);
    }

    public void GoToOpening()
    {
        GoTo(SCENE_OPENING);
    }

    public void GoToGame()
    {
        GoTo(SCENE_GAME, true);
    }

    public void GoToIntro()
    {
        GoTo(SCENE_INTRO);
    }

    public void GoToCredits()
    {
        GoTo(SCENE_CREDITS);
    }

    public void GoToHelp()
    {
        GoTo(SCENE_HELP);
    }

    public void Exit()
    {
        if (cc != null) StopCoroutine(cc);
        cc = StartCoroutine(IEQuit());
    }

    private void GoTo(string sceneName, bool hard = false)
    {
        if (cc != null) StopCoroutine(cc);
        cc = StartCoroutine(IETransition(sceneName, hard));
    }
    private IEnumerator IETransition(string sceneName, bool hard = false)
    {
        if (PauseService.IsInstantiated) PauseService.Inst.Locked = false;
        //yield return new WaitForSecondsRealtime(Delay);
        yield return StartCoroutine(FadeToBlack());
        yield return new WaitForSecondsRealtime(Delay);
        if (hard)
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            Resources.UnloadUnusedAssets();
        }
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

    }
    private IEnumerator IEQuit()
    {        
        yield return StartCoroutine(FadeToBlack());
        Application.Quit();
    }
    private Coroutine cc;

    [Button]
    public void Start()
    {        
        cc = StartCoroutine(FadeFromBlack());              
    }

    [Button]
    public void FadeToBlackTest()
    {        
        StartCoroutine(FadeToBlack());
    }

    public IEnumerator FadeToBlack()
    {        
        var c = GetComponentInChildren<CanvasGroup>();
        yield return Coroutines.AnimateFrames(
            FadeToBlackDuration,
            (float t) => {
                c.alpha = Mathf.Pow(t, TimePower);
            },
            unscaled:true,
            reverse:false
        );
    }

    public IEnumerator FadeFromBlack()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(SceneLoadDelay);
        var c = GetComponentInChildren<CanvasGroup>();
        yield return Coroutines.AnimateFrames(
            FadeFromBlackDuration,
            (float t) => {
                c.alpha = Mathf.Pow(t, TimePower);                
            },
            unscaled: true,
            reverse: true
        );
    }
}
