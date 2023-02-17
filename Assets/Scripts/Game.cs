using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : Singleton<Game>
{
    const string SCENE_GAME = "Game";
    public GameObject GameOverPanel;
    private bool isOver = false;
    public void GameOver()
    {
        if (isOver) return;
        isOver = true;
        
        var lang = (I18n.Inst.CurrentLanguage == "English") ? "EN" : "UA";
        SFXManager.Inst.Play($"Voice.GameOver.{lang}");
        PauseService.Inst.PausePanel = GameOverPanel.gameObject;
        PauseService.Inst.Pause(nosound:true);
        PauseService.Inst.Locked = true;
    }

    public void Restart()
    {
        
        PauseService.Inst.Locked = false;
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        Resources.UnloadUnusedAssets();
        isOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        //SceneManager.U
        //SceneManager.LoadScene(SCENE_GAME,LoadSceneMode.Single);
    }

    public void Exit()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
    void Start()
    {
        isOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
