using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(100000)]
public class EmmissAnim : MonoBehaviour
{
    [ColorUsage(true, true)]
    public Color Emission;
    
    public bool clamp = false;
    public bool blockUnityEditorMaterialSave = true;
    
    
    public float AnimationSpeed = 0.3f;
    public float Pow = 10;

#if UNITY_EDITOR
    private Material materialCopyToRestore;
#endif

    public Renderer Renderer;

    private void Start()
    {
        #if UNITY_EDITOR
        materialCopyToRestore = new Material(Renderer.sharedMaterial);
        #endif
        Renderer.sharedMaterial.EnableKeyword("_EMISSION");        
    }

    void OnApplicationQuit()
    {
#if UNITY_EDITOR
        if (blockUnityEditorMaterialSave)  Renderer.sharedMaterial.CopyPropertiesFromMaterial(materialCopyToRestore);
#endif
    }

    [ExecuteInEditMode]
    void Update()
    {
        var sin = Mathf.Sin(Time.time * AnimationSpeed);
        var phase = clamp ? Mathf.Clamp(sin, 0, 1) : Mathf.Abs(sin);
        phase = Mathf.Pow(phase, Pow);

        Renderer.sharedMaterial.SetColor("_EmissionColor", Emission * phase);
    }
}
