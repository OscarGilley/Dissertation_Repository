using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Outdated Script, intended functionality implemented in TerrainGenerator.cs
public class CreateCollider : MonoBehaviour
{

    public MeshFilter meshfilter;

    // Start is called before the first frame update
    void Start()
    {

       // CreateTerrainCollider( meshfilter,edgeCollider);


    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void CreateTerrainColliderr(MeshFilter meshfilter, EdgeCollider2D edgeCollider)
    {
        //meshfilter = GetComponent<MeshFilter>();

        Vector3[] vertices3D = meshfilter.mesh.vertices;
        Vector2[] vertices2D = new Vector2[vertices3D.Length / 2];

        int j = 0;


        for (int i = 0; i < vertices3D.Length; i += 4)
        {
            vertices2D[j] = vertices3D[i + 2];

            j++;

            vertices2D[j] = vertices3D[i + 3];

            j++;


        }

        //3rd and 4th of every tri are the surface level coords, use edge collider?

        //EdgeCollider2D edgeCollider = GetComponent<EdgeCollider2D>();
        edgeCollider.points = vertices2D;



    }
}