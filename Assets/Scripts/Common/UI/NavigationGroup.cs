using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NavigationGroup : Selectable
{
    
    public override void OnSelect(BaseEventData eventData)
    {
        StartCoroutine(SelectChild());
    }

    IEnumerator SelectChild()
    {
        yield return new WaitForEndOfFrame();
        GetComponentsInChildren<Toggle>().First(t => t.isOn).Select();
    }
}
