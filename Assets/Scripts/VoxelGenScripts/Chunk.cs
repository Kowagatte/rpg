using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Chunk : MonoBehaviour
{
    private Block [ , , ] blocks = new Block[chunkSize, chunkSize, chunkSize];

    public World world;
    public WorldPos pos;

    public static readonly int chunkSize = 16;
    public bool update = true;

    private MeshFilter filter;
    private MeshCollider coll;

    private void Start()
    {
        filter = gameObject.GetComponent<MeshFilter>();
        coll = gameObject.GetComponent<MeshCollider>();
    }

    public Block GetBlock(int x, int y, int z)
    {
        if(InRange(x) && InRange(y) && InRange(z)) return blocks[x, y, z];

        return world.GetBlock(pos.x + x, pos.y + y, pos.z + z);
    }

    public void SetBlock(int x, int y, int z, Block block)
    {
        if( InRange(x) && InRange(y) && InRange(z))
        {
            blocks[x, y, z] = block;
        }
        else
        {
            world.SetBlock(pos.x + x, pos.y + y, pos.z + z, block);
        }
    }

    public static bool InRange(int index)
    {
        if( index < 0 || index >= chunkSize ) return false;

        return true;
    }

    private void Update()
    {
        if(update)
        {
            update = false;
            UpdateChunk();
        }
    }

    private void UpdateChunk()
    {
        MeshData meshData = new MeshData();

        for( int x = 0; x < chunkSize; x++ )
        {
            for( int y = 0; y < chunkSize; y++ )
            {
                for( int z = 0; z < chunkSize; z++ )
                {
                    meshData = blocks[x, y, z].Blockdata(this, x, y, z, meshData);
                }
            }
        }

        RenderMesh(meshData);
    }

    private void RenderMesh(MeshData meshData)
    {
        filter.mesh.Clear();
        filter.mesh.vertices = meshData.vertices.ToArray();
        filter.mesh.triangles = meshData.triangles.ToArray();
        filter.mesh.uv = meshData.uv.ToArray();
        filter.mesh.RecalculateNormals();

        coll.sharedMesh = null;
        Mesh mesh = new Mesh();
        mesh.vertices = meshData.colVertices.ToArray();
        mesh.triangles = meshData.colTriangles.ToArray();
        mesh.RecalculateNormals();

        coll.sharedMesh = mesh;
    }
}
