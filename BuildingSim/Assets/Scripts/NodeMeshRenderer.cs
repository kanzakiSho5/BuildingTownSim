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

        var triangles = new List<int> {0, 1, 2, 0, 2, 3};
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
    }

    public void CreateMesh(List<Vector3> vertex)
    {
        mesh = new Mesh();

        var uvs = new List<Vector2>();
        var triangles = new List<int>();
        foreach (var vert in vertex)
        {
            uvs.Add(new Vector2(vert.x * .1f,vert.z * .1f));
        }

        for (var i = 0; i <= vertex.Count - 2; i++)
        {
            triangles.Add(0);
            triangles.Add(i + 1);
            triangles.Add(i);
        }
        
        mesh.SetVertices(vertex);
        mesh.SetUVs(0,uvs);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
    }
}
