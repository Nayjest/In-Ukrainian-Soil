using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVoicing : MonoBehaviour
{
    private string lang;
    private float lastTime = -15;
    private float iterT = 4;
    private Coroutine c;
    void Start()
    {
        lang = (I18n.Inst.CurrentLanguage == "English") ? "EN" : "UA";
        //lang = "UA";
        StartCoroutine(Process());
    }
    IEnumerator Process()
    {
        var startDelay = Random.Range(5f, 30f);
        yield return new WaitForSeconds(startDelay);
        SFXManager.Inst.Play($"Voice.Tip.{lang}");
        while (true)
        {
            yield return new WaitForSeconds(iterT);
            var timer = Player.Inst.Timer;
            if (timer < 13 && timer > 1 && TimePassed(20))
            {
                lastTime = Time.unscaledTime;
                SFXManager.Inst.Play($"Voice.LowEnergy.{lang}");
            }
        }
        yield return null;
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    bool TimePassed(float t)
    {
        return Time.unscaledTime - lastTime > t;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
