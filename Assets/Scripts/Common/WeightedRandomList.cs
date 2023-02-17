
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class WeightedRandomList<T>
{

    [System.Serializable]
    public class Wrapper
    {
        public int Weight = 1;

        [ShowAssetPreview]
        public T Element;

        private int currentWeight = -1;
        public int ActualWeight => currentWeight == -1 ? Reset() : currentWeight;

        public int ChangeWeight(int requiredDelta)
        {
            var w = ActualWeight;
            int delta;
            if (requiredDelta < 0)
            {
                delta = w > requiredDelta ? requiredDelta : w;
            }
            else
            {
                delta = w + requiredDelta <= Weight ? requiredDelta : Weight - w;
            }
            currentWeight += delta;
            return delta;
        }

        public int Reset()
        {
            return currentWeight = Weight;
        }
    }

    private int wSum = 0;
    public int WeightSum => wSum == 0 ? RefreshWeightSum() : wSum;

    public System.Action OnResetWeights = () => { };
    public System.Action OnSequenceEnded = () => { };

    private int lastIndex = -1;

    [AllowNesting]
    [ShowIf("ExposeFieldsToInspector")]
    public int WeightDecrease = 0;
    
    [AllowNesting]
    [ShowIf("ExposeFieldsToInspector")]
    public int WeightIncrease = 0;

    [AllowNesting]
    [ShowIf("ExposeFieldsToInspector")]
    public bool DropZeroWeightElements = false;

    [AllowNesting]
    [ShowIf("ExposeFieldsToInspector")]
    public int WeightsResetLimit = 0;

    
    [AllowNesting][HideInInspector]
    public bool ExposeFieldsToInspector = true;


    [SerializeField]
    private List<Wrapper> elements;

    public RandomUtils.Seed Seed = RandomUtils.Seed.None;
    public List<Wrapper> Elements
    {
        get
        {
            if (elements == null) elements = new List<Wrapper>();
            return elements;
        }
        set
        {
            elements = value.Select(e => new Wrapper { Element = e.Element, Weight = e.Weight }).ToList();
            wSum = 0;
            lastIndex = -1;
        }
    }
    private int RefreshWeightSum()
    {
        return wSum = Elements.Sum(e => e.ActualWeight);
    }

    public WeightedRandomList() { }
    public WeightedRandomList(List<Wrapper> newElements)
    {
        Elements = newElements;
    }

    public T GetWeightedRandom()
    {        
        var r = RandomUtils.Range(0, WeightSum + 1, Seed);
        var i = 0;
        foreach (var e in elements)
        {
            i += e.ActualWeight;
            if (r <= i && e.ActualWeight != 0)
            {
                return ExtractElement(e);
            }
        }
        if (elements == null || elements.Count == 0) throw new System.Exception($"Trying to choose element from empty weighted random list");
        throw new System.Exception($"Error choosing element from weighted random list");
    }

    private void RandomizeOrder()
    {
        RandomUtils.SetSeed(Seed);
        elements = elements.OrderBy(x => Random.value).ToList();
        RandomUtils.RestoreSeed();
    }


    public T GetByOrder(bool randomizeSeq = false)
    {
        lastIndex++;
        if (lastIndex >= elements.Count || lastIndex < 0) // lastIndex < 0 -- in case of deserialization 
        {
            lastIndex = 0;
            OnSequenceEnded.Invoke();
        }
        if (randomizeSeq && lastIndex == 0) RandomizeOrder();
        return ExtractElement(elements[lastIndex]);
    }
    private void IncreaseAllWeights()
    {
        if (WeightIncrease != 0)
        {
            foreach (var oe in Elements) oe.ChangeWeight(WeightIncrease);
            RefreshWeightSum();
        }
    }
    private T ExtractElement(Wrapper e)
    {
        if (WeightDecrease != 0)
        {
            IncreaseAllWeights();
            var delta = e.ChangeWeight(-WeightDecrease);
            wSum += delta;
            if (DropZeroWeightElements && e.ActualWeight == 0)
            {
                Elements.Remove(e);
            }
            if (wSum <= WeightsResetLimit) ResetAllWeights();
        }
        return e.Element;
    }

    private void ResetAllWeights()
    {
        foreach (var e in Elements) e.Reset();
        RefreshWeightSum();
        OnResetWeights.Invoke();
    }
}
