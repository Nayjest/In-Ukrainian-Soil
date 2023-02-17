using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "FontReplacement", menuName = "FontReplacement", order = 2)]
public class FontReplacement : ScriptableObject
{
    public SystemLanguage L;

    [System.Serializable]
    public class Replacement: AbstractReplacement<TMP_FontAsset>
    {
        public float FontSizeMultiplier = 1;

        // DO NOT USE IT IF YOU HAVE BOLD TEXTS
        public bool Bold = false;
        public void ReplaceFont(TMP_Text text)
        {
            if (LanguageSpecific.name == text.font.name) return;                    
            text.font = LanguageSpecific;
            text.fontSize *= FontSizeMultiplier;
            text.fontSizeMax *= FontSizeMultiplier;
            if (Bold) text.fontStyle = FontStyles.Bold;
        }

        public void Revert(TMP_Text text)
        {
            text.font = Original;
            text.fontSize /= FontSizeMultiplier;
            text.fontSizeMax /= FontSizeMultiplier;            
            if (Bold) text.fontStyle &= ~FontStyles.Bold;
        }
    }

    [System.Serializable]
    public class AbstractReplacement<T>
    {
        public string Language;
        public T Original;
        public T LanguageSpecific;        
        public bool Enabled;
    }

    [System.Serializable]
    public class CoreReplacement: AbstractReplacement<Font>
    {
        public void ReplaceFont(Text text)
        {
            if (LanguageSpecific.name == text.font.name) return;
            text.font = LanguageSpecific;
        }

        public void Revert(Text text)
        {
            text.font = Original;            
        }
    }

    public Replacement[] replacements;
    public CoreReplacement[] coreReplacements;

    public List<Replacement> ReplacementsByLang(string lang)
    {
        return replacements.Where(r => r.Language == lang).ToList();
    }

    public List<CoreReplacement> CoreReplacementsByLang(string lang)
    {
        return coreReplacements.Where(r => r.Language == lang).ToList();
    }

}
