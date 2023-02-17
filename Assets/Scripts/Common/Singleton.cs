/**
 * © 2022 Vitalii Stepanenko
 * Licensed under the MIT License
 * 
 * Base Singleton class
 * 
 * Features:
 *  - Can be loaded with object from resources (may be useful if you need some configuration of singleton instance made in editor, but dont want to store it in each scene)
 *  - Can be placed to scene as regular Monobehaviour 
 *  - Can be instantiated with DontDestroyOnLoad if persistance between scenes needed
 * 
 * Usage Example: 
 * <code>
 * // All attributes are optionsl
 * [SingletonSetup(ContainerName ="GameObjectNameHere", PrefabName ="FileInResourcesFolder", InstanceName ="MyClass")]
 * public class MyClass : Singleton<MyClass>
 * {
 *  
 *     protected void Awake()
 *     {
 *         base.Awake();      
 *         // Do stuff here
 *     }
 * }
 * 
 * // To get instance anywhere in code:
 * var myInstance = MyClass.Inst  
 * </code>
 */
using UnityEngine;

#region [SingletonSetup Attribute implementation]
public class SingletonSetupAttribute : System.Attribute
{
    /// <summary>
    /// Name for GameObject storing singleton, optional
    /// </summary>
    public string InstanceName;

    /// <summary>
    /// Name of prefab in Assets/Resources folder without filename extension, for preloading GameObject from resources
    /// </summary>
    public string PrefabName;

    /// <summary>
    /// Name of GameObject that will be parent for singleton instance, optional
    /// </summary>
    public string ContainerName;
    public bool DontDestroyOnLoad = false;

    public Transform ContainerTransform
    {
        get
        {
            return ContainerName == null ? null : GameObject.Find(ContainerName).transform;
        }
    }
    public GameObject Prefab
    {
        get
        {
            if (PrefabName == null) return null;
            var o = Resources.Load<GameObject>(PrefabName);
            if (o == null)
            {
                throw new System.Exception($"Can't load singleton prefab, name:{PrefabName}");
            }
            return o;
        }
    }

}
# endregion

#region [Singleton Implementation]
public class Singleton<T> : MonoBehaviour
    where T : MonoBehaviour
{

    private static T _inst;

    public static bool IsInstantiated { get { return _inst != null; } }

    private static SingletonSetupAttribute setupAttribute()
    {
        var attrs = typeof(T).GetCustomAttributes(typeof(SingletonSetupAttribute), false);
        if (attrs.Length > 0) return (SingletonSetupAttribute)attrs[0];

        //Debug.LogWarning($"Singleton {typeof(T)}: SingletonSetup attribute not found");

        var a = new SingletonSetupAttribute();
        a.InstanceName = typeof(T).ToString();
        return a;
    }

    private static T instantiate()
    {
        var setup = setupAttribute();

        var prefab = setup.Prefab;

        GameObject o;
        if (prefab == null)
        {
            o = new GameObject(setup.InstanceName);
            o.transform.parent = setup.ContainerTransform;
        }
        else
        {
            o = GameObject.Instantiate(setup.Prefab, setup.ContainerTransform);            
            if (setup.InstanceName != null) o.name = setup.InstanceName;
        }

        if (setup.DontDestroyOnLoad)
        {
            DontDestroyOnLoad(o);
        }
        var c = o.GetComponent<T>();
        if (c == null) c = o.AddComponent<T>();
        return c;
    }

    public static T GetInstance() { return Inst; }
    public static T Inst
    {
        get
        {
            if (_inst == null)
            {
                _inst = GameObject.FindObjectOfType<T>() ?? instantiate();
            }
            return _inst;
        }
    }

    /// <summary>
    /// Grants possibility to have singleton instance in scene.
    /// 
    /// Dont forget to call base.Awake() if you will override it.
    /// </summary>
    protected virtual void Awake()
    {
        if (_inst == null)
        {
            _inst = gameObject.GetComponent<T>();
        }
        else
        {
            if (_inst != this) throw new System.Exception(
                $"Singleton instance placed to scene, but another instance already loaded. " +
                $"Class: {this.GetType().ToString()}, " +
                $"preloaded instance name: {_inst.gameObject.name}, " +
                $"second instance name:{gameObject.name}"
            );
        }
    }
}
#endregion