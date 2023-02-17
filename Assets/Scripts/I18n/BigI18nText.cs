using NaughtyAttributes;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BigI18nText : MonoBehaviour
{    
    [System.Serializable]
    public class TranslatedText
    {
        public SystemLanguage Language;

        [AllowNesting]
        [ResizableTextArea]
        public string Text;
    }

    [AllowNesting]
    public List<TranslatedText> Texts;
    void Awake()
    {
        var l = UserPrefs.Inst.Settings.Language;
        var t = GetComponent<TMP_Text>();
        foreach (var i in Texts)
        {
            if (i.Language == l)
            {
                t.text = i.Text;
                I18n.Inst.FontFor(t.font)?.ReplaceFont(t);                
                break;
            }
        }
    }
}
