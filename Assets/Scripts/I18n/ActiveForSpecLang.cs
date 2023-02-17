using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveForSpecLang : MonoBehaviour
{
    public List<SystemLanguage> Languages;
    // Start is called before the first frame update
    void Start()
    {
        var currentLang = UserPrefs.Inst.Settings.Language;
        if (!Languages.Contains(currentLang)) GameObject.Destroy(gameObject);
    }
}
