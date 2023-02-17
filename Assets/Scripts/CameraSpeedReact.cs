using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpeedReact : MonoBehaviour
{
    public List<Camera> Cameras;
    public Vector2 MinMaxFOV = new Vector2(100, 150);
    public float ZoomSpeed = 1;
    public float Pow = 0.75f;

    public float T => Mathf.Pow(Mathf.Clamp(Player.Inst.CurrentVelocity - 15, 0, 100) / 100f, Pow);
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        float v = Player.Inst.CurrentVelocity;
        v = v - 20;
        float t = Mathf.Clamp(v, 0, 100) / 100f;
        t = Mathf.Pow(t, 0.7f);
        */
        var targetFov = Mathf.Lerp(MinMaxFOV.x, MinMaxFOV.y, T);
        foreach (var c in Cameras)
        {
            c.fieldOfView = Mathf.Lerp(c.fieldOfView, targetFov, ZoomSpeed);
        }
    }
}
