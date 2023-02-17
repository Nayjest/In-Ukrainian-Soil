using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.VFX;

public class Impact : MonoBehaviour
{
    public AudioSource Src;
    public AudioClip Clip;
    public Vector2 MinMaxPitch = Vector2.one;
    public VisualEffect VFX;
    public GameObject ImpactPrefab;
    private void Reset()
    {
        Src = GetComponent<AudioSource>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (Clip)
        {
            Src.pitch = MinMaxPitch.Random();
            Src.PlayOneShot(Clip);
        }
        /*
        if (VFX)
        {
            VFX.transform.position = collision.contacts[0].point;
            VFX.Reinit();
            VFX.Play();
        }
        */
        if (ImpactPrefab)
        {
            var o = GameObject.Instantiate(ImpactPrefab);
            o.transform.position = collision.contacts[0].point;            
        }
    }
}
