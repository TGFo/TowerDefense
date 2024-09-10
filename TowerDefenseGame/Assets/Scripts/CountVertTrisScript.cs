using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountVertTrisScript : MonoBehaviour
{
    public TMP_Text text;
    public MeshGenerator meshGenerator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "Triangles: " + meshGenerator.GetTriangleCount().ToString() + "\nVertices: " + meshGenerator.GetVertCount().ToString();
    }
}
