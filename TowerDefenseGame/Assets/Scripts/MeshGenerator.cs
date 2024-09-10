using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class MeshGenerator : MonoBehaviour
{
    [Header("Debug Variables")]
    public bool autoUpdate;
    public bool drawGizmos;

    Mesh mesh;
    MeshFilter meshFilter;
    MeshCollider meshCollider;
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uv;

    [Header("Generation Parameters")]
    public int xSize = 20;
    public int zSize = 20;

    public float noiseScale = 1;
    public float heightMultiplier = 1;
    public float multiplier = 1;
    public float noisePosOffset = 0;

    public bool randomNoiseScale = true;
    public bool randomOffset = true;
    public bool vertexAlignedPaths = false;

    public Vector3 targetPoint;
    private List<Vector3[]> paths = new List<Vector3[]>();
    private List<Vector3> allPaths = new List<Vector3>();
    List<GameObject> placePointObjects = new List<GameObject>();

    public GameObject mouseOverPrefab;
    public GameObject placeableIndicator;
    List<Vector3> placementPoints = new List<Vector3>();

    //[Header("Texture Parameters")]
    //int rows = 5;
    //int columns = 10;
    void Start()
    {
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        meshFilter.mesh = mesh;
        GenerateMesh(randomOffset, randomNoiseScale);
        
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.R))
        {
            //regenerates mesh if tower is not placed
            if(ResourceManager.instance.towerPlaced == false)
            {
                GenerateMesh(randomOffset, randomNoiseScale);
                Debug.Log("regenerating terrain");
                Debug.Log(GetVertexIndex(targetPoint));
            }
        }
    }
    public void GenerateMesh(bool randomNoiseOffset = true, bool randomNoiseScale = true)
    {
        Debug.Log("Generating mesh");
        if(randomNoiseOffset )
        {
            noisePosOffset = Random.Range(-5, 5);
        }
        if(randomNoiseScale)
        {
            noiseScale = Random.Range(0.0f, 1.0f);
        }
        CreateShape(noiseScale);
        targetPoint = vertices[vertices.Length/2];
        UpdateMesh();
    }
    void CreateShape(float noiseValue)
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        uv = new Vector2[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * noiseScale + noisePosOffset, z * noiseScale + noisePosOffset) * heightMultiplier;
                vertices[i] = new Vector3(x * multiplier, y * multiplier, z * multiplier);
                //uv[i] = new Vector2(((float)x / xSize), ((float)z / zSize));
                i++;
            }
        }
        GenerateUvs();
        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }
    //Loops through the vertices setting UVs
    public void GenerateUvs()
    {
        int uvRemainder = (zSize + 1) % 2;  //get remainder of division by 2
        Debug.Log(uvRemainder);
        for (int y = 0, i = 0; y <= (zSize / 2) - uvRemainder; y++) //runs for half the mesh
        {
            for (int x = 0; x <= xSize; x++)    //loops through vertices row by row setting uvs 
            {
                Vector2 uvPos = new Vector2(1, 0);
                Vector2 uvPosUp = new Vector2(1, 1);
                if (vertices[i].y == 0) //sets path UVs
                {
                    uvPos.x = 0.45f;
                    uvPosUp.x = 0.45f;
                }
                if (i % 2 == 0) //checks if current position is even, setting UVs accordingly
                {
                    uvPos.x = 0.55f;
                    uvPosUp.x = 0.55f;
                    if (vertices[i].y == 0)
                    {
                        uvPos.x = 0f;
                        uvPosUp.x = 0f;
                    }
                }
                uv[i] = uvPos;
                uv[i + xSize + 1] = uvPosUp;
                i++;
            }
            i += xSize + 1; //moves up a row
        }
    }
    //Sets the target point for pathgen to the tower location
    public void AddTower(Vector3 target)
    {
        targetPoint = target;
        GeneratePaths(3);
    }
    //Generates a user defined number of paths
    public void GeneratePaths(int numPaths)
    {
        paths.Clear();
        for(int i = 0; i < numPaths; i++)
        {
            Vector3[] currentPath = GeneratePath(GetRandomEdge(), targetPoint);
            paths.Add(currentPath);
            FlattenPath(currentPath, 0);
        }
        foreach(Vector3[] path in paths)
        {
            allPaths.AddRange(path);
        }
        placementPoints.AddRange(GenerateDefensePoints(allPaths.ToArray(), 3f, 25));
        UpdateMesh();
    }
    public Vector3 GetRandomEdge()
    {
        List<Vector3> edges = EdgeVertices();
        return edges[Random.Range(0, edges.Count)];
    }
    public void UpdateMesh()
    {
        GenerateUvs();
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        meshCollider.sharedMesh = mesh;
    }
    //Generates path
    public Vector3[] GeneratePath(Vector3 startPoint, Vector3 endPoint)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 currentPoint = startPoint;
        Vector3 nextPoint;
        path.Add(currentPoint);

        int maxIterations = 1000; // Failsafe to prevent infinite loops
        int iteration = 0;

        while (Vector3.Distance(currentPoint, endPoint) > multiplier && iteration < maxIterations)
        {
            Vector3 direction = (endPoint - currentPoint).normalized;

            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

            nextPoint = currentPoint + ((randomOffset * 0.5f) + direction).normalized * multiplier;

            if(vertexAlignedPaths)
            {
                nextPoint = GetClosestVertPosition(nextPoint);
            }
            else
            {
                nextPoint.x = Mathf.Clamp(nextPoint.x, 0, xSize * multiplier);
                nextPoint.z = Mathf.Clamp(nextPoint.z, 0, zSize * multiplier);

                // Optionally round to keep points aligned to the grid
                nextPoint.x = Mathf.Round(nextPoint.x);
                nextPoint.z = Mathf.Round(nextPoint.z);
            }
            nextPoint.y = 0;
            path.Add(nextPoint);
            currentPoint = nextPoint;
            iteration++;
        }

        path.Add(endPoint);

        // Debugging log
        Debug.Log("Generated path with " + path.Count + " points");

        return path.ToArray();
    }
    //Gets the index of a specified vert
    public int GetVertexIndex(Vector3 vertex)
    {
        int xIndex = Mathf.RoundToInt(vertex.x / multiplier);
        int zIndex = Mathf.RoundToInt(vertex.z / multiplier);

        xIndex = Mathf.Clamp(xIndex, 0, xSize);
        zIndex = Mathf.Clamp(zIndex, 0, zSize);

        int vertexIndex = zIndex * (xSize + 1) + xIndex;

        if(vertexIndex < 0 || vertexIndex >= vertices.Length)
        {
            vertexIndex = 0;
        }

        return vertexIndex;
    }
    //Gets the index of a specified UV
    public int GetUvIndex(Vector2 vertex)
    {
        int xIndex = Mathf.RoundToInt(vertex.x / multiplier);
        int yIndex = Mathf.RoundToInt(vertex.y / multiplier);

        xIndex = Mathf.Clamp(xIndex, 0, xSize);
        yIndex = Mathf.Clamp(yIndex, 0, zSize);

        int vertexIndex = yIndex * (xSize + 1) + xIndex;

        if (vertexIndex < 0 || vertexIndex >= vertices.Length)
        {
            vertexIndex = 0;
        }

        return vertexIndex;
    }
    //Flattens the quads surrouding the path
    public void FlattenPath(Vector3[] path, float pathHeight = 0, bool padding = true)
    {
        for (int i = 0; i < path.Length; i++)
        {
            int pathIndex = GetVertexIndex(path[i]);
            vertices[GetVertexIndex(path[i])].y = pathHeight;
            uv[pathIndex] = new Vector2(0.4f, 0.5f);
            int[] paddingIndexes =
            {
                Mathf.Clamp(pathIndex + 1, 0, vertices.Length - 1), //right vert
                Mathf.Clamp(pathIndex - 1, 0, vertices.Length - 1), //left vert

                Mathf.Clamp(pathIndex + 1 + (xSize + 1), 0, vertices.Length - 1),   //right up vert
                Mathf.Clamp(pathIndex + 1 - (xSize + 1), 0, vertices.Length - 1),   //right down vert
                Mathf.Clamp(pathIndex - 1 + (xSize + 1), 0, vertices.Length - 1),   //left up vert
                Mathf.Clamp(pathIndex - 1 - (xSize + 1), 0, vertices.Length - 1),   //left down vert

                Mathf.Clamp(pathIndex + (xSize + 1), 0, vertices.Length - 1),   //up vert
                Mathf.Clamp(pathIndex - (xSize + 1), 0, vertices.Length - 1)    //down vert

            };
            if (padding == true)
            {
                foreach (int index in paddingIndexes)
                {
                    vertices[index].y = pathHeight;
                    uv[index] = new Vector2(.4f, 0.5f);
                }
            }
        }

    }
    //Generates points where player can place defenders near path
    public List<Vector3> GenerateDefensePoints(Vector3[] path, float distanceFromPath = 3, int numberOfPoints = 5)
    {
        List<Vector3> randomPoints = new List<Vector3>();

        for (int i = 0; i < numberOfPoints; i++)
        {
            Vector3 randomPathVertex = path[Random.Range(0, path.Length)];
            Vector2 randomDirection2D = Random.insideUnitCircle.normalized;
            Vector3 randomPoint = new Vector3();
            randomPoint.x = randomPathVertex.x + randomDirection2D.x * distanceFromPath;
            randomPoint.y = randomPathVertex.y;
            randomPoint.z = randomPathVertex.z + randomDirection2D.y * distanceFromPath;
            randomPoint.x = Mathf.Clamp(randomPoint.x, 0, xSize * multiplier);
            randomPoint.z = Mathf.Clamp(randomPoint.z, 0, zSize * multiplier);

            //Ensure point is not placed on path
            if (GetClosestVertPosition(randomPoint).y != 0)
            {
                randomPoints.Add(GetClosestVertPosition(randomPoint));
            }
            else i--; 
        }
        return randomPoints;
    }
    //checks if a defender can be placed on the selected vert
    public bool IsPlacable(Vector3 placePosition)
    {
        bool isPlacable = false;
        foreach (Vector3 point in placementPoints)
        {
            if(point == placePosition)
            {
                return true;
            }
        }
        return isPlacable;
    }
    //gets a list of the surrounding edges
    public List<Vector3> EdgeVertices()
    {
        List<Vector3> edgeVertices = new List<Vector3>();

        for(int x = 0; x <= xSize; x++)
        {
            edgeVertices.Add(vertices[x]);
            edgeVertices.Add(vertices[vertices.Length - 1 - x]);
        }
        for(int z = 0; z <= zSize; z++)
        {
            edgeVertices.Add(vertices[z * (xSize + 1)]);
            edgeVertices.Add(vertices[z * (xSize + 1) + xSize]); 
        }
        return edgeVertices;
    }
    //finds the closest vertex to the selected position
    public Vector3 GetClosestVertPosition(Vector3 pos)
    {
        int index = GetVertexIndex(pos);
        return vertices[index];
    }
    //gets a random path
    public Vector3[] GetRandomPath()
    {
        return paths[Random.Range(0, 3)];
    }
    //Highlights the closest vert to mouse position
    public void HighlightVert(Vector3 pos)
    {
        Vector3 vertPos = GetClosestVertPosition(pos);
        mouseOverPrefab.transform.position = vertPos;
    }
    //Highlights defender place points
    public void HighLightPlacePoints()
    {
        GameObject indicator;
        foreach (Vector3 pos in placementPoints)
        {
            indicator = Instantiate(placeableIndicator);
            indicator.transform.position = pos;
            placePointObjects.Add(indicator);
        }
    }
    //Hides highlight when defender placed
    public void HidePlacementPoint(Vector3 pos)
    {
        if(placePointObjects.Count == 0)
        {
            return;
        }
        foreach(GameObject placement in placePointObjects)
        {
            if(pos == placement.transform.position)
            {
                placement.SetActive(false);
            }
        }
    }
    //Sets the target for path generation
    public void SetTargetPoint(Vector3 pos)
    {
        targetPoint = pos;
    }
    public int GetVertCount()
    {
        return vertices.Count();
    }
    public int GetTriangleCount()
    {
        return triangles.Count();
    }
    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }
        if(drawGizmos == false)
        {
            return;
        }
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
        Gizmos.color = Color.red;
        foreach (Vector3[] path in paths)
        {
            for (int i = 0; i < path.Length - 1; i++)
            {
                Gizmos.DrawLine(path[i], path[i + 1]);
                Gizmos.DrawCube(path[i], new Vector3(.1f, .1f, .1f));
            }
        }
        Gizmos.color = Color.green;
        foreach (Vector3 point in placementPoints)
        {
            Gizmos.DrawSphere(point, .1f);
        }
        Gizmos.DrawSphere(targetPoint, 0.2f);
    }
}
