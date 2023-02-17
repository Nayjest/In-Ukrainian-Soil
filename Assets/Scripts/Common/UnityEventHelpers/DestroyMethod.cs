using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMethod : MonoBehaviour
{
    // Start is called before the first frame update
    public void Destroy()
    {
        GameObject.Destroy(gameObject);
    }
}
