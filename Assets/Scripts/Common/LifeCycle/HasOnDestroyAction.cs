
using UnityEngine;

public class HasOnDestroyAction : MonoBehaviour
{
    public System.Action OnDestroyAction;
    void OnDestroy()
    {
        if (OnDestroyAction != null) OnDestroyAction.Invoke();
    }
}