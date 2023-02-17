using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TxtScore : MonoBehaviour
{
    public TMP_Text Txt;

    private int score;
    // Update is called once per frame
    void Update()
    {
        if (score != Player.Inst.Score)
        {
            score = Player.Inst.Score;
            Txt.text = score.ToString();
        }        
    }
}
