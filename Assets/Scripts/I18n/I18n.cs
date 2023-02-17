using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using TMPro;
using NaughtyAttributes;


//https://www.unitytutorials.ca/systems/unity-localization-using-po
public class I18n : Singleton<I18n>
{
    //This is where the current loaded language will go
    private Hashtable textTable;

    public Dictionary<SystemLanguage, string> SupportedLanguages = new Dictionary<SystemLanguage, string>
    {
        {SystemLanguage.Ukrainian, "Українська" },
        {SystemLanguage.English, "English" },
    };

    public FontReplacement FontReplacement;

    private void OnEnable()
    {
        Init();
        //StartCoroutine(TranslateAll());
    }
    IEnumerator TranslateAll()
    {
        //yield return new WaitForEndOfFrame();
        UpdateAllTextBoxes();
        yield break;
    }

    [ReadOnly]
    //Just a reference for the current language, default to english
    public string CurrentLanguage = "English";

    [ShowNativeProperty]
    public string L => CurrentLanguage;

    [ReadOnly]
    [SerializeField]
    private List<FontReplacement.Replacement> replacedFonts;

    [ReadOnly]
    [SerializeField]
    private List<FontReplacement.CoreReplacement> replacedCoreFonts;

    public FontReplacement.Replacement FontFor(TMP_FontAsset orig)
    {
        if (replacedCoreFonts == null) return null;
        foreach (var i in replacedFonts)
        {
            if (i.Enabled && i.Original.name == orig.name)
            {
                return i;
            }
        }
        return null;
    }

    public FontReplacement.CoreReplacement FontFor(Font orig)
    {
        foreach (var i in replacedCoreFonts)
        {
            if (i.Enabled && i.Original.name == orig.name)
            {
                return i;
            }
        }
        return null;
    }

    void InitFontReplacement()
    {
        FontReplacement = Resources.Load<FontReplacement>("I18n/FontReplacement");
        if (FontReplacement == null) return;
        replacedFonts = FontReplacement.ReplacementsByLang(CurrentLanguage);
        replacedCoreFonts = FontReplacement.CoreReplacementsByLang(CurrentLanguage);        
    }
    //Run this when you are ready to start the language process, you usually want to do this after everything has loaded
    public void Init()
    {
        //You should use an enum for storing settings that are a unique list,
        //This gets a string representation of an enum,  
        CurrentLanguage = GetLanguageString(UserPrefs.Inst.Settings.Language);
        InitFontReplacement();
        
        //Pass that language into LoadLanguage, remember we are in init so this should only run once.
        LoadLanguage(CurrentLanguage);
    }

    public string GetLanguageString(SystemLanguage lang)
    {
        return Enum.GetName(typeof(SystemLanguage), (int)lang);
    }

    //You call this when you want to update all text boxes with the new translation.
    //Run this after Init
    //Run this whenever you run LoadLanguage
    //Run this whenever you load a new scene and want to translate the new UI
    public void UpdateAllTextBoxes()
    {        
        //Find all active and inactive text boxes and loop through 'em
        I18nText[] temp = Resources.FindObjectsOfTypeAll<I18nText>();
        Debug.Log("I18n.UpdateAllTextBoxes, qty:" + temp.Length);
        foreach (I18nText text_box in temp)
        {           
            // Do not modify prefabs
            if (text_box.gameObject.scene.IsValid() == false) continue;            
            text_box.UpdateTranslation();
        }
    }

    //Run this whenever a language changes, like in when a setting is changed - then run UpdateAllTextBoxes
    //This is based off of http://wiki.unity3d.com/index.php?title=TextManager, though heavily modified and expanded
    private void LoadLanguage(string lang)
    {        
        CurrentLanguage = lang;

        if (lang == "English")
        {
            UpdateAllTextBoxes();
        }
        else if (lang != "English")
        {
            string fullpath = "I18n/" + lang + ".po"; // the file is actually ".txt" in the end

            TextAsset textAsset = (TextAsset)Resources.Load(fullpath);
            if (textAsset == null)
            {
                Debug.LogError("[I18n] " + fullpath + " file not found.");
                return;
            }
            else
            {
                Debug.Log("[I18n] loading: " + fullpath);

                if (textTable == null)
                {
                    textTable = new Hashtable();
                }

                textTable.Clear();

                StringReader reader = new StringReader(textAsset.text);
                string key = null;
                string val = null;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.StartsWith("msgid \""))
                    {
                        key = line.Substring(7, line.Length - 8).ToUpper();
                        key = key.Replace("<BR>", "\n");
                    }
                    else if (line.StartsWith("msgstr \""))
                    {
                        val = line.Substring(8, line.Length - 9);
                        val = val.Replace("<br>", "\n");
                    }
                    else
                    {
                        if (key != null && val != null)
                        {
                            // TODO: add error handling here in case of duplicate keys
                            textTable.Add(key, val);
                            key = val = null;
                        }
                    }
                }                
                reader.Close();
            }
        }
    }

    //This handles selecting the value from the translation array and returning it, the UILocalizeText calls this
    public string GetText(string text)
    {
        if (text != null && textTable != null)
        {
            var key = text.ToUpper();
            if (textTable.ContainsKey(key))
            {
                return (string)textTable[key];
            }

        }
        return text;
    }

    public static void Translate(GameObject o, string newText = null)
    {
        GetInstance();
        var t = o.GetComponent<I18nText>();
        if (!t) t = o.AddComponent<I18nText>();
        t.UpdateTranslation(newText);
    }
}