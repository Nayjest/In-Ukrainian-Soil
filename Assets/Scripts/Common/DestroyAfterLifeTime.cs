using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterLifeTime : MonoBehaviour
{
    private const string DESTROY_METHOD = "SelfDestroy";
    public bool RandomizeLifeTime = false;

    [HideIf("RandomizeLifeTime")]
    public float Lifetime = 1f;

    [ShowIf("RandomizeLifeTime")]
    public Vector2 MinMaxLifeTime;

    void Start()
    {
        var lt = RandomizeLifeTime ? Random.Range(MinMaxLifeTime.x, MinMaxLifeTime.y) : Lifetime;
        Invoke(DESTROY_METHOD, Lifetime);
    }

    public void SelfDestroy()
    {
        if (this != null && gameObject != null)  GameObject.Destroy(gameObject);
    }
}
