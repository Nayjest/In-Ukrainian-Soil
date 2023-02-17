using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class FollowMouse : MonoBehaviour
{
    public Vector2 Offset = Vector2.zero;
    public Camera Camera;
    void Start()
    {
        if (Camera == null) Camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        var mousePosition = Input.mousePosition;        
        mousePosition = Camera.ScreenToWorldPoint(mousePosition);
        transform.position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z) + Offset.ToVector3(); 
    }
}
