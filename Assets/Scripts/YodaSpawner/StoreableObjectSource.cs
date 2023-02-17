using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "_SRC", menuName = "ObjectSource")]
public class StoreableObjectSource : ScriptableObject
{
    public ObjectSource Data = new ObjectSource();
    void Reset()
    {
        Data = new ObjectSource();
    }
}
