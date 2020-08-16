using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class NodeMeshRenderer : MonoBehaviour
{
    Mesh mesh;
    MeshFilter meshFilter;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
    }

    private void Start()
    {

        var vertices = new List<Vector3>
        {
            new Vector3(-2f, .1f, -2f),
            new Vector3(-2f, .1f, 2f),
            new Vector3(2f, .1f, 2f),
            new Vector3(2f, .1f, -2f),
        };
        mesh.SetVertices(vertices);

        var uvs = new List<Vector2>
        {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(.1f, 1),
            new Vector2(.1f, 0)
        };
        mesh.SetUVs(0,uvs);

        var triangles = new List<int> {0, 1, 2, 2, 3, 0};
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
    }

    public void CreateMesh(List<Vector3> position)
    {
        
    }
}
