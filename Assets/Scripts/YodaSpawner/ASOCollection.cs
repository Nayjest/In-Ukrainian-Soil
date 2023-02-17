
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public abstract class ASOCollection<T>:ScriptableObject
{
   
    public List<WeightedRandomList<T>.Wrapper> Elements = new List<WeightedRandomList<T>.Wrapper>();
    public WeightedRandomList<T> GetWeightedRandomList()
    {
        return new WeightedRandomList<T>(Elements);
    }
}
