using TMPro;
using UnityEngine;

public class UseLangFont : MonoBehaviour
{
    public SystemLanguage Language;
    void Start()
    {
        var text = GetComponent<TMP_Text>();        
        foreach (var i in I18n.Inst.FontReplacement.ReplacementsByLang(I18n.Inst.GetLanguageString(Language)))
        {
            if (i.Enabled && i.Original.name == text.font.name)
            {
                i.ReplaceFont(text);
                break;
            }
        }
    }
}
