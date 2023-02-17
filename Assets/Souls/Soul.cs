using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Soul : MonoBehaviour
{
    public GameObject Root;
    private void OnCollisionEnter(Collision collision)
    {
        //Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().FindSoul();
            GameObject.Destroy(Root);
        }
    }

    public void CleanRocks()
    {
        var Removed = Physics.OverlapSphere(transform.position, (0.5f + 0.1f) * transform.localScale.x)
            .Select(x => x.gameObject)
            .Where(o => o.tag == "EnvObject" && o.activeSelf)
            .ToList();
        foreach (var o in Removed)
        {
            o.SetActive(false);
        }
    }
    private void OnEnable()
    {
        Invoke("CleanRocks", 0.01f);
    }


}
