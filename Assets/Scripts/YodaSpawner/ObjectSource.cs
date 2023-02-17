using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectSource : ISerializationCallbackReceiver
{

    public enum ChooseMethod
    {
        WeightedRandom,
        DynamicWeightedRandom,
        Sequence,
        RandomizedSequence,
        Pool,
    }

    public bool IsDynamicWeightedRandom => Method == ChooseMethod.DynamicWeightedRandom && !IsSingleObject;

    public bool IsPool => Method == ChooseMethod.Pool && !IsSingleObject;

    public enum StorageTypes
    {
        ExternalCollection,
        LocalCollection,
        SingleObject
    }

    [AllowNesting]
    public StorageTypes StorageType = StorageTypes.ExternalCollection;

    [SerializeField] [AllowNesting][ShowIf("IsSingleObject")]
    private Object SingleObject;
    
    bool IsExternalCollection => StorageType == StorageTypes.ExternalCollection;
    bool IsSingleObject => StorageType == StorageTypes.SingleObject;    
    bool IsLocalCollection => StorageType == StorageTypes.LocalCollection;

    [AllowNesting]    
    [ShowIf(EConditionOperator.Or, "IsExternalCollection", "IsLocalCollection")]
    public ChooseMethod Method = ChooseMethod.WeightedRandom;


    [AllowNesting]    
    [ShowIf("IsDynamicWeightedRandom")]
    public int WeightDecrease = 0;

    [AllowNesting]
    [ShowIf("IsDynamicWeightedRandom")]    
    public int WeightIncrease = 0;

    [AllowNesting]
    [ShowIf(EConditionOperator.Or, "IsPool", "IsDynamicWeightedRandom")]
    public int WeightsResetLimit = 0;

    [AllowNesting]
    //[Expandable]
    [ShowIf(EConditionOperator.And, "IsExternalCollection")]
    public ObjectCollection Collection;

    [AllowNesting]
    [SerializeField]    
    [ShowIf(EConditionOperator.And, "IsLocalCollection", "EditorRecreateList")]
    private WeightedRandomList<Object> _list;

    private bool EditorRecreateList()
    {
        if (IsLocalCollection)
        {
            if (_list != null && _list.ExposeFieldsToInspector) { _list = null; }
            Initialize();
        }
        return true;
    }

    public System.Action OnPoolExhausted { 
        get { return list.OnResetWeights; }
        set { list.OnResetWeights = value; }
    }

    public System.Action OnSequenceEnded
    {
        get { return list.OnSequenceEnded; }
        set { list.OnSequenceEnded = value; }
    }

    private WeightedRandomList<Object> list
    {
        get
        {
            if (_list == null)
            {
                Initialize();
            }
            return _list;
        }
    }    

    private void CreateListIfEmpty()
    {
        if (_list != null && _list.Elements != null && _list.Elements.Count > 0) return;
        if (StorageType == StorageTypes.ExternalCollection)
        {            
            if (Collection != null)
            {
                _list = Collection.GetWeightedRandomList();                
            } else
            {
                //Debug.LogError("Using Object Source With empty collection");
            }
        }
        if (StorageType == StorageTypes.LocalCollection)
        {
            _list = new WeightedRandomList<Object>() { ExposeFieldsToInspector = false };
        }
    }
    public void Initialize()
    {        
        if (StorageType == StorageTypes.SingleObject) return;
        CreateListIfEmpty();

        list.WeightsResetLimit = WeightsResetLimit;
        if (Method == ChooseMethod.Pool)
        {
            list.WeightDecrease = 1;
            list.WeightIncrease = 0;
            list.WeightsResetLimit = WeightsResetLimit;
        }
        else if (Method == ChooseMethod.DynamicWeightedRandom)
        {
            list.WeightDecrease = WeightDecrease;
            list.WeightIncrease = WeightIncrease;
            list.WeightsResetLimit = WeightsResetLimit;
        }
    }


    protected Object ReturnValue(Object val)
    {
        if (val is StoreableObjectSource)
        {
            return ((StoreableObjectSource)val).Data.Choose();
        }
        return val;
    }
        
    public Object Choose()
    {
        if (StorageType == StorageTypes.SingleObject) return ReturnValue(SingleObject);
        if (list.Elements.Count == 0)
        {
            Debug.LogError("ObjectSource: Trying to choose object from empty list  "+(IsExternalCollection? "External Collection":"Local Collection"));
            Debug.LogError(Collection.GetWeightedRandomList().Elements.Count);
            return null;
        }
        switch (Method)
        {
            case ChooseMethod.WeightedRandom:
            case ChooseMethod.DynamicWeightedRandom:
            case ChooseMethod.Pool:
                return ReturnValue(list.GetWeightedRandom());
            case ChooseMethod.Sequence:
                return ReturnValue(list.GetByOrder());
            case ChooseMethod.RandomizedSequence:
                return ReturnValue(list.GetByOrder(randomizeSeq: true));
        }
        throw new System.Exception("ChooseMethod not set");
    }

    public void OnBeforeSerialize()
    {
        if (Application.isPlaying) return;
        if (IsSingleObject)
        {
            Collection = null;
            _list = null;
        }
        if (IsExternalCollection)
        {
            SingleObject = null;
            _list = null;
        }
        if (IsLocalCollection)
        {
            Collection = null;
            SingleObject = null;
        }        
    }

    public void OnAfterDeserialize()
    {
        //if (!Application.isPlaying) return;
        //Initialize();
    }
}
