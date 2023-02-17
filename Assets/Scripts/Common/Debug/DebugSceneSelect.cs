using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class DebugSceneSelect : MonoBehaviour
{
    private GameObject root;

    void Start()
    {        
        root = new GameObject("DebugCanvas");        
        var c = root.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        root.AddComponent<GraphicRaycaster>(); // Required to interact with UI
        var dd = DefaultControls.CreateDropdown(new DefaultControls.Resources()).GetComponent<Dropdown>();
        dd.transform.SetParent(c.transform, false);        
        dd.ClearOptions();
        var names = new List<string>() { "== Select Scene ==" };
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            names.Add(Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)));
        }
        dd.AddOptions(names);
        dd.onValueChanged.AddListener((i) => { SceneManager.LoadScene(dd.options[i].text); });
        root.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3) && Input.GetKey(KeyCode.LeftAlt)) root.SetActive(!root.activeSelf);
        if (Input.GetKeyDown(KeyCode.F3) && Input.GetKey(KeyCode.LeftShift)) SceneManager.LoadScene("GameNoHUD");
        if (Input.GetKeyDown(KeyCode.F3) && Input.GetKey(KeyCode.RightShift)) SceneManager.LoadScene("LaserNebenzia");
    }
}