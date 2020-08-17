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
