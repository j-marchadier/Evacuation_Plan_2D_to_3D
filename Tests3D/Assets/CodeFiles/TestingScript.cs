using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeDeeBear.Models.Ply;

public class TestingScript : MonoBehaviour
{
    MeshFilter m_Mf;
    [SerializeField] string plypath;
    // Start is called before the first frame update
    void Start()
    {
        m_Mf = GetComponent<MeshFilter>();

        PlyResult res = PlyHandler.GetVerticesAndTriangles(plypath);
        List<Vector3> vertices = res.Vertices;
        List<int> triangles = res.Triangles;

        Mesh mesh = new Mesh();
        mesh.name = "strip";

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        
        m_Mf.sharedMesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
