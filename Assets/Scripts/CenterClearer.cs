using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;

[DefaultExecutionOrder(1000000)]
public class CenterClearer : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> Removed = new List<GameObject>();    
    private void OnCollisionEnter(Collision collision)
    {
        Removed.Add(collision.gameObject);
        collision.gameObject.SetActive(false);
        
    }

    private void Start()
    {
        Invoke("Check", 0.00001f);
    }
    [Button]
    private void Check()
    {    
        Removed = Physics.OverlapSphere(Vector3.zero, 0.5f * transform.localScale.x)
            .Select(x => x.gameObject)
            .Where(o => o.tag != "Player" && o.activeSelf)
            .ToList();
        foreach(var o in Removed)
        {
            o.SetActive(false);
        }
        USoil.Space.Inst.OnQuadrantChange.AddListener((Vector3Int dir) => {
            foreach (var o in Removed) o.SetActive(true);
            //GameObject.Destroy(gameObject);
        });
    }
}
