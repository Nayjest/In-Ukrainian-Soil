using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    public List<Sprite> Sprites;
    void Start()
    {
        var sr = GetComponent<SpriteRenderer>();
        sr.sprite = Sprites[Random.Range(0, Sprites.Count)];
    }
}
