// MeshData.cs
using System.Collections.Generic;
using UnityEngine;

public class MeshData {
    public Vector3[] Vertices { get; }
    public int[] Triangles { get; }
    //public Color32[] Colors { get; }

    public MeshData(Vector3[] vertices, int[] triangles) {
        Vertices = vertices;
        Triangles = triangles;
        //Colors = colors;
    }
}