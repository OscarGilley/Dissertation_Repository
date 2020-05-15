using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

//Script Adapted from referenced online resource, LARGELY MODIFIED, areas where the original code is largely unmodified are noted
//Resource Link: https://www.codementor.io/nickwiggill/eye-openers-series-generating-and-smoothing-2d-terrain-in-unity-p2fpsd0zm

public class TerrainGenerator : MonoBehaviour
{
    public Mesh mesh;
    public MeshFilter meshFilter;
    public const int MIN = 1;
    public const int MAX = 30;
    public const int LENGTH = 101;
    public const int SCAN_RADIUS = 2;
    public const int complexity = 3;
    public bool firstGen = true;
    public EdgeCollider2D edgeCollider;
    public GameObject platform;
    public List<GameObject> platforms;
    public GameObject enemy;
    public List<GameObject> enemies;
    public List<float> times;


    float[] heightmap = new float[LENGTH];

    //ran every frame
    void Update()
    {
        /*if (firstGen)
        {
            Debug.Log("Level Size: " + LENGTH + "\nDiffiuclty: " + complexity);
            BuildLevel();
            firstGen = false;  
        }
        */

        //Used for gathering data.
        if (Input.GetKeyDown(KeyCode.T))
        {

            float[] avgScores = new float[LENGTH / 10];

            for (int i = 0; i < 100; i++)
            {
                float timeB = 0;
                timeB = Time.realtimeSinceStartup;
                mesh.Clear();
                

                mesh = new Mesh();
                mesh.name = "Terrain mesh";
                meshFilter.mesh = mesh;


                for (int j = 0; j < platforms.Count; j++)
                {
                    Destroy(platforms[j]);
                }

                for (int k = 0; k < enemies.Count; k++)
                {
                    Destroy(enemies[k]);
                }

                platforms.Clear();
                enemies.Clear();

                UnityEngine.Random.InitState(UnityEngine.Random.Range(1, 1000));
                for (int l = 0; l < LENGTH; l++)
                {

                    heightmap[l] = UnityEngine.Random.Range(MIN, MAX);
                }

                BuildLevel();

                float timeA = 0;
                
                timeA = Time.realtimeSinceStartup;

                float duration = (timeA - timeB) * 1000;

                UnityEngine.Debug.Log("Time in millis: " + duration);

                times.Add(duration);

                //float[] scores = DifficultyScore(meshFilter);

                //for(int j = 0; j < avgScores.Length; j++)
                //{
                //    avgScores[j] = avgScores[j] + scores[j];
                //}

            }

            float total = 0;
            float maximum = 0;
            float minimum = int.MaxValue;

            for(int m = 0; m < times.Count; m++)
            {
                total += times[m];

                if(times[m] > maximum)
                {
                    maximum = times[m];
                }

                if(times[m] < minimum)
                {
                    minimum = times[m];
                }
            }

            //Debug.Log(times);
            UnityEngine.Debug.Log("Number of Generations: " + times.Count);
            UnityEngine.Debug.Log("Level Size: " + LENGTH + "\nDifficulty: " + complexity);
            UnityEngine.Debug.Log("Average Time: " + total / times.Count + "ms");
            
            UnityEngine.Debug.Log("Maximum Time: " + maximum + "ms");
            UnityEngine.Debug.Log("Minimum Time: " + minimum + "ms");

            //for(int k = 0; k < avgScores.Length; k++) 
            //{
            //    UnityEngine.Debug.Log("Average score for chunk " + k + " is " + avgScores[k] / 100);
            //}
            
        }

        //Used to refresh the level.
        if (Input.GetKeyDown(KeyCode.Q))
        {
            float timeB = 0;
            timeB = Time.realtimeSinceStartup;
            mesh.Clear();


            mesh = new Mesh();
            mesh.name = "Terrain mesh";
            meshFilter.mesh = mesh;


            for (int j = 0; j < platforms.Count; j++)
            {
                Destroy(platforms[j]);
            }

            for (int k = 0; k < enemies.Count; k++)
            {
                Destroy(enemies[k]);
            }

            platforms.Clear();
            enemies.Clear();

            UnityEngine.Random.InitState(UnityEngine.Random.Range(1, 1000));
            for (int l = 0; l < LENGTH; l++)
            {

                heightmap[l] = UnityEngine.Random.Range(MIN, MAX);
            }

            BuildLevel();

            float timeA = 0;

            timeA = Time.realtimeSinceStartup;

            float duration = (timeA - timeB) * 1000;

            UnityEngine.Debug.Log("Time in millis: " + duration);

            times.Add(duration);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            float total = 0;
            float maximum = 0;
            float minimum = int.MaxValue;

            for (int m = 0; m < times.Count; m++)
            {
                total += times[m];

                if (times[m] > maximum)
                {
                    maximum = times[m];
                }

                if (times[m] < minimum)
                {
                    minimum = times[m];
                }
            }

            //Debug.Log(times);
            UnityEngine.Debug.Log("Amount of Results: " + times.Count);
            UnityEngine.Debug.Log("Level Size: " + LENGTH + "\nDifficulty: " + complexity);
            UnityEngine.Debug.Log("Average Time: " + total / times.Count + "ms");
            UnityEngine.Debug.Log("Maximum Time: " + maximum + "ms");
            UnityEngine.Debug.Log("Minimum Time: " + minimum + "ms");
        } 

        if (Input.GetKeyDown(KeyCode.Z))
        {
            float[] scores = DifficultyScore(meshFilter);

            for (int i = 0; i < scores.Length; i++)
            {
                UnityEngine.Debug.Log("Score for chunk " + i + " is " + scores[i]);
            }
        }
        

    }

    void BuildLevel()
    {
        for (int i = 0; i < 10; i++)
        {

            Smooth();

            firstGen = false;
        }

        for (int i = 0; i < LENGTH; i += 100)
        {



            System.Random rnd = new System.Random();

            float pitStart = rnd.Next(16, LENGTH - 16);
            float pitWidth = rnd.Next(3, 9);

            for (int j = (int)pitStart; j < pitStart + pitWidth; j++)
            {
                heightmap[j] = 0;
            }

        }

        BuildMesh();
    }

    //smooths out the heightmap of the terrain, adapated from online resource
    void Smooth()
    {
        for (int i = 0; i < heightmap.Length; i++)
        {
            //float height = heightmap[i];

            float heightSum = 0;
            float heightCount = 0;


            for (int n = i - SCAN_RADIUS; n < i + SCAN_RADIUS + 1; n++)
            {
                if (n >= 0 && n < heightmap.Length)
                {
                    float heightOfNeighbour = heightmap[n];
                    heightSum += heightOfNeighbour;
                    heightCount++;
                }
            }

            if (heightmap[i] != 0)
            {
                float heightAverage = heightSum / heightCount;
                heightmap[i] = heightAverage;
            }
        }
    }

    //runs on startup
    void Awake()
    {
        UnityEngine.Debug.Log("Level Size: " + LENGTH + "\nDifficulty: " + complexity);
        mesh = new Mesh();
        mesh.name = "Terrain mesh";
        meshFilter.mesh = mesh;


        for (int i = 0; i < platforms.Count; i++)
        {
            Destroy(platforms[i]);
        }

        for (int j = 0; j < enemies.Count; j++)
        {
            Destroy(enemies[j]);
        }

        platforms.Clear();
        enemies.Clear();

        UnityEngine.Random.InitState(UnityEngine.Random.Range(1, 1000));
        for (int k = 0; k < LENGTH; k++)
        {

            heightmap[k] = UnityEngine.Random.Range(MIN, MAX);
        }

        BuildLevel();


    }

    //builds a mesh for the level's terrain, adapted from online resource
    void BuildMesh()
    {
        mesh.Clear();

        List<Vector3> positions = new List<Vector3>();
        List<int> triangles = new List<int>();

        int offset = 0;
        for (int i = 0; i < LENGTH - 1; i++)
        {

            offset = i * 4; //offsets the generation of triangles so they don't all overlap each other
                            // * 4 since 4 vertices are generated each time

            float h = heightmap[i];
            float hn = heightmap[i + 1];

            Vector3 bottomLeft = new Vector3(i + 0, 0, 0);
            Vector3 bottomRight = new Vector3(i + 1, 0, 0);
            Vector3 topLeft = new Vector3(i + 0, h, 0);
            Vector3 topRight = new Vector3(i + 1, hn, 0);

            if (topRight.y == 0 && topLeft.y != 0)
            {
                topLeft.y = 0;
            }

            if (topLeft.y == 0 && topRight.y != 0)
            {
                topRight.y = 0;
            }

            positions.Add(bottomLeft); //bottom left
            positions.Add(bottomRight); //bottom right
            positions.Add(topLeft); //top left
            positions.Add(topRight); //top right, corner uses the bottom left y coord of next tri, smooth terrain

            triangles.Add(offset + 0);
            triangles.Add(offset + 2);
            triangles.Add(offset + 1);

            triangles.Add(offset + 1);
            triangles.Add(offset + 2);
            triangles.Add(offset + 3);
        }


        mesh.vertices = positions.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        CreateTerrainCollider(meshFilter, edgeCollider);
        CreatePlatform(meshFilter);
        CreateEnemies(meshFilter);
    }
    void CreateTerrainCollider(MeshFilter meshfilter, EdgeCollider2D edgeCollider)
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

        edgeCollider = GetComponent<EdgeCollider2D>();
        edgeCollider.points = vertices2D;



    }

    void CreatePlatform(MeshFilter meshfilter)
    {
        for (int k = 0; k < 5 - complexity; k++)
        {


            for (int i = 0; i < (meshfilter.mesh.vertices.Length) / 400; i++)
            {
                int startX = UnityEngine.Random.Range((i * 100) + 15, (i * 100) + 85);
                //int endX = startX + UnityEngine.Random.Range(6, 10);


                float height = 0;
                

                //Debug.Log(startX);
                if (meshfilter.mesh.vertices[(startX * 4) + 2].y != 0)
                {
                    height = meshfilter.mesh.vertices[(startX * 4) + 2].y + 5;

                }

                else
                {
                    height = meshfilter.mesh.vertices[((startX - 10) * 4) + 2].y + 5;

                }

                GameObject instance = Instantiate(platform, new Vector3(startX, height, 0), Quaternion.identity) as GameObject;

                platforms.Add(instance);
            }
        }
    }

    void CreateEnemies(MeshFilter meshfilter)
    {
        for (int k = 0; k < Math.Pow(2, complexity - 1); k++)
        {
            for (int i = 0; i < (meshfilter.mesh.vertices.Length) / 400; i++)
            {
                int startX = UnityEngine.Random.Range((i * 100) + 15, (i * 100) + 85);


                float height = 0;

                if (meshfilter.mesh.vertices[(startX * 4) + 2].y != 0)
                {
                    height = meshfilter.mesh.vertices[(startX * 4) + 2].y + 1;
                }

                else
                {
                    startX = startX - 10;
                    height = meshfilter.mesh.vertices[((startX) * 4) + 2].y + 1;
                }



                GameObject instance = Instantiate(enemy, new Vector3(startX, height, 0), Quaternion.identity) as GameObject;

                enemies.Add(instance);
            }
        }
    }

    float[] DifficultyScore(MeshFilter meshFilter)
    {

        float[] scores = new float[LENGTH / 10];

        UnityEngine.Debug.Log("scores size: " + scores.Length);

        for (int i = 0; i < LENGTH - 10; i+=10)
        {
            int chunkStart = i;
            int chunkEnd = 10 + i;
            

            float chunkScore = 0.0f;

            for(int k = 0; k < enemies.Count; k++)
            {
                int count = 0;

                if (enemies[k].transform.position.x > chunkStart && enemies[k].transform.position.x <= chunkEnd)
                {             
                    chunkScore = chunkScore + (1 + count);
                    count++;
                }
            }

            for(int j = 0; j < platforms.Count; j++)
            {
                int count = 0;
                if (platforms[j].transform.position.x > chunkStart && platforms[j].transform.position.x <= chunkEnd)
                {
                    chunkScore = chunkScore - (1 + count);
                    count++;
                }
            }


            for (int m = 0; m < 10; m++)
            {
                if(meshFilter.mesh.vertices[(i * 4) + (m * 4) + 2].y == 0 && meshFilter.mesh.vertices[(i * 4) + (m * 4) + 2].x > chunkStart && meshFilter.mesh.vertices[(i * 4) + (m * 4) + 2].x <= chunkEnd)
                {
                    chunkScore = chunkScore + 0.5f;
                }

                if (meshFilter.mesh.vertices[(i * 4) + (m * 4) + 3].y == 0 && meshFilter.mesh.vertices[(i * 4) + (m * 4) + 3].x > chunkStart && meshFilter.mesh.vertices[(i * 4) + (m * 4) + 3].x <= chunkEnd)
                {
                    chunkScore = chunkScore + 0.5f;
                }
            }
           
            
            scores[i / 10] = chunkScore;

        }

        return scores;
    }
}