using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class RndRotate : MonoBehaviour
{
    public Vector3 Rotation;
    public float MaxAngleSpeed = 300;
    public bool RandomizeX = true;
    public bool RandomizeY = true;
    public bool RandomizeZ = true;
    public bool ExecuteInEditor = false;

    public bool RandomizeOverTime = false;
    public float RandomiseOverTimeInterval = 3;

    Vector3 MakeRnd => new Vector3(
            RandomizeX ? Random.Range(-MaxAngleSpeed, MaxAngleSpeed) : Rotation.x,
            RandomizeY ? Random.Range(-MaxAngleSpeed, MaxAngleSpeed) : Rotation.y,
            RandomizeZ ? Random.Range(-MaxAngleSpeed, MaxAngleSpeed) : Rotation.z
        );

    void Start()
    {

        Rotation = MakeRnd;
        
        if (RandomizeOverTime)
        {
            StartCoroutine(IERandomizeOverTime());
        }
    }

    IEnumerator IERandomizeOverTime()
    {
        while (true)
        {
            var next = MakeRnd;
            var prev = Rotation;
            yield return StartCoroutine(Coroutines.AnimateFrames(RandomiseOverTimeInterval, (t) => {
                Rotation = Vector3.Lerp(prev, next, t);
            }));            
        }        
    }

    // Update is called once per frame
    void Update()
    {
        if (ExecuteInEditor || Application.isPlaying)
        {
            transform.Rotate(Rotation * Time.deltaTime);
        }
    }


    private void OnEnable()
    {      
#if UNITY_EDITOR
        EditorApplication.update += Update;
#endif
    }

    void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.update -= Update;
#endif
    }

}
