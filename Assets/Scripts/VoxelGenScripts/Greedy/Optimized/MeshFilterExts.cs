using UnityEngine;

public static class MeshFilterExts 
{
    public static void ApplyMeshData(this MeshFilter meshFilter, MeshData meshData) {
        meshFilter.mesh.Clear();
        meshFilter.mesh.vertices = meshData.Vertices;
        meshFilter.mesh.triangles = meshData.Triangles;

        //Color mesh and calculate normals
        //meshFilter.mesh.colors32 = meshData.Colors;
        meshFilter.mesh.RecalculateNormals();
    }
}
