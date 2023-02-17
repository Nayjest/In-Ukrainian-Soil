using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnyKeyToStart : MonoBehaviour
{
    public bool ToMenu = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (ToMenu)
                SceneTransitions.Inst.GoToMenu();
            else
                SceneTransitions.Inst.GoToGame();
        }
    }
}
