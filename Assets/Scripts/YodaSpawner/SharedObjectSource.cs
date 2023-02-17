using NaughtyAttributes;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-250)]
[SingletonSetup(InstanceName ="SharedObjectSource", PrefabName ="SharedObjectsSource")]
public class SharedObjectSource : Singleton<SharedObjectSource>
{
    [System.Serializable]
    public class NamedEntity
    {
        public string Name;
        public ObjectSource Src;

    }

    public List<NamedEntity> Sources = new List<NamedEntity>();

    private Dictionary<string, NamedEntity> dict;

    protected override void Awake()
    {
        Sources.ForEach(i => i.Src.Initialize());
        dict = Sources.ToDictionary(i => i.Name);
        base.Awake();
    }

    [Button]
    public void NewWithLocalCollection()
    {
        var src = new ObjectSource();
        src.Collection = ScriptableObject.CreateInstance<ObjectCollection>();
        Sources.Add(new NamedEntity{ Name ="_NEW", Src = src });
    }

    public ObjectSource GetByName(string name)
    {
        if (!dict.ContainsKey(name)) throw new System.Exception($"Trying to access unexisting Shared Object Source, name = {name}");
        return dict[name].Src;
    }

    public IEnumerable<string> Names => Sources.Select(i=>i.Name);
}
