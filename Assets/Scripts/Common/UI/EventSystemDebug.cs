using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemDebug : MonoBehaviour
{
    [ShowNativeProperty]
    public string CurrentSelected => EventSystem.current?.currentSelectedGameObject?.name ?? "";
   
}
