using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class I18nText : MonoBehaviour
{

    //This instances key, as well as the english translation
    private string TranslationKey = "";
    
    private TMP_Text text;
    private Text coreText;

    private TMP_FontAsset origFont;

    private FontReplacement.Replacement r;
    private FontReplacement.CoreReplacement coreR;

    public bool CreatedDynamically = false;    

    //References to text values
    string OriginalText = "";

    public bool LogError = false;
    //This gets run automatically if the original text hasn't been set when you go to update it. 
    //You shouldn't need to manually run this from anywhere
    public void Init()
    {
        //Grab the TextToTranslate if we haven't
        if (text == null)
            text = GetComponent<TMP_Text>();
        if (text == null)
            coreText = GetComponent<Text>();

        //Grab the original value of the text before we update it
        if (text != null)
            OriginalText = text.text;
        
        if (coreText != null)
            OriginalText = coreText.text;

        //Set the translation key to the original english text
        if (TranslationKey == "")
            TranslationKey = OriginalText;
    }

    public void Start()
    {
        if (CreatedDynamically) UpdateTranslation();
    }
    private void OnEnable()
    {
        //UpdateTranslation();
    }

    //This gets called from LanguageManager
    //One thing I noticed is that it might be nicer to just pass in the correct string to this rather than go grap it from LanguageManager
    public void UpdateTranslation(string newText = null)
    {
        //If original text is empty, then this object hasn't been initiated so it should do that 
        if (OriginalText == "")
        Init();
        if (newText != null)
        {
            OriginalText = newText;
            TranslationKey = newText;
        }
        if (LogError) Debug.LogError($"Translation Key: '{TranslationKey}', Language: " + I18n.Inst.CurrentLanguage + ", Translation: " + I18n.Inst.GetText(TranslationKey));
        //If the object has no Text object then we shouldn't try to set the text so just stop
        if ((text == null) && (coreText == null)) {
            Debug.LogError("I18nText compoennt without TMP_Text: " + gameObject.name + "/"+gameObject.transform.parent.name);
            return;
        };

        if (I18n.Inst.CurrentLanguage != "English" && TranslationKey != "")
        {
            string new_text = I18n.Inst.GetText(TranslationKey);            
            if (new_text != "")
            {                
                if (text)
                {
                    if (r!=null) r.Revert(text);
                    var re = I18n.Inst.FontFor(text.font);
                    if (re != null)
                    {
                        re.ReplaceFont(text);
                        r = re;
                    }                                
                    text.text = new_text;
                } else
                {
                    if (coreR != null) coreR.Revert(coreText);
                    var re = I18n.Inst.FontFor(coreText.font);
                    if (re != null)
                    {
                        re.ReplaceFont(coreText);
                        coreR = re;
                    }                    
                    coreText.text = new_text;
                }
            }
            else
            {
                // Debug.Log("Key " + OriginalText + " doesn't have an entry in this language");
            }
        }
        /** @todo  looks like  it's never used*/
        else if (I18n.Inst.CurrentLanguage == "English")
        {
            if ((r != null) && (text != null) && (r.Original != null))
            {
                r.Revert(text);                
                r = null;
            }

            if ((coreR != null) && (coreText != null) && (coreR.Original != null))
            {
                coreR.Revert(coreText);
                coreR = null;
            }
            /*
            if (OrigFont != null)
            {
                Text.font = OrigFont;            
            }
            */
            if ((text != null) && (OriginalText != null))
                text.text = OriginalText;
            if ((coreText != null) && (OriginalText != null))
                coreText.text = OriginalText;
        }

    }

}