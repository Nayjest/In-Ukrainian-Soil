using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LangSwitch : Singleton<LangSwitch>
{
    public void UA()
    {
        UserPrefs.Inst.Settings.Language = SystemLanguage.Ukrainian;
        UserPrefs.Inst.WriteData();
        I18n.Inst.Init();
        I18n.Inst.UpdateAllTextBoxes();
        SceneTransitions.Inst.GoToOpening();
    }

    public void EN()
    {
        UserPrefs.Inst.Settings.Language = SystemLanguage.English;
        UserPrefs.Inst.WriteData();
        I18n.Inst.Init();
        I18n.Inst.UpdateAllTextBoxes();
        SceneTransitions.Inst.GoToOpening();
    }
}
