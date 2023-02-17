using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GptGen1 : MonoBehaviour
{

    //public List<Transform> Src;

    public List<Vector3> points => transform.Cast<Transform>().Select(t => t.position).ToList();
   
    //Src.Select(x => x.transform.position).ToList(); 
    //list of points that define the curve


    
    public int numSides = 10; //number of sides for the cylinder

    public float radius = 1f; //radius of the cylinder

    void Start()
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh mesh = filter.mesh;

        mesh.Clear();

        Vector3[] vertices = new Vector3[points.Count * numSides];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[(points.Count - 1) * numSides * 6];

        float angleStep = 2f * Mathf.PI / numSides; //angle between two sides of the cylinder

        for (int i = 0; i < points.Count; i++)
        {
            for (int j = 0; j < numSides; j++)
            {
                float angle = j * angleStep; //angle of current side 

                //calculate position of vertex on current side 
                vertices[i * numSides + j] = points[i] + radius * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

                uvs[i * numSides + j] = new Vector2((float)j / numSides, (float)i / points.Count);

                if (i > 0 && j > 0)
                {
                    int t1 = (i - 1) * numSides + (j - 1);
                    int t2 = (i - 1) * numSides + j;
                    int t3 = i * numSides + (j - 1);
                    int t4 = i * numSides + j;

                    triangles[(i - 1) * numSides * 6 + 6 * (j - 1)] = t1;
                    triangles[(i - 1) * numSides * 6 + 6 * (j - 1) + 1] = t2;
                    triangles[(i - 1) * numSides * 6 + 6 * (j - 1) + 2] = t3;

                    triangles[(i - 1) * numSides * 6 + 6 * (j - 1) + 3] = t2;
                    triangles[(i - 1) * numSides * 6 + 6 * (j - 1) + 4] = t4;
                    triangles[(i - 1) * numSides * 6 + 6 * (j - 1) + 5] = t3;
                }
            }
        }
        mesh.vertices = vertices; mesh.uv = uvs; mesh.triangles = triangles; mesh.RecalculateNormals();
    }

    

}