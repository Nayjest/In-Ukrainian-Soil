using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionFeedback : MonoBehaviour
{
    public List<GameObject> RotationCenters;
    public List<Renderer> Renderers;
    public List<Transform> HeadPoints;
    public float Duration = 0.3f;
    public float TPower = 1.5f;
    [ColorUsage(true, true)]
    public Color DefaultColor;
    [ColorUsage(true, true)]
    public Color Color;
    public float HeadOffsetMultiplier = 10f;

    void StartEffect()
    {
        StopAllCoroutines();
        StartCoroutine(Coroutines.AnimateFrames(Duration, (t) => {
            t = Mathf.Pow(t, TPower);
            var c = Color.Lerp(Color, DefaultColor, t);
            foreach(var r in Renderers)
            {
                r.material.SetColor("_Glow", c);
            }
        }));

        StartCoroutine(Coroutines.AnimateFrames(Duration, (t) => {

            var scale = Mathf.Lerp(HeadOffsetMultiplier, 1, t);
            foreach (var r in HeadPoints)
            {
                r.localScale = Vector3.one * scale;
            }
        }));
    }
    private void OnCollisionEnter(Collision collision)
    {
        StartEffect();

    }

    void Start()
    {
        DefaultColor = Renderers[0].material.GetColor("_Glow");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
