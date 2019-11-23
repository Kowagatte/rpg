using UnityEngine;

/// <summary>
/// Enum for Directions :^)
/// </summary>
public enum Dir
{
    South,
    North,
    East,
    West,
    Top,
    Bottom
}


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class WorldGen : MonoBehaviour
{
    public int VoxelSize = 1;
    public int ChunkWidth = 3;
    public int ChunkHeight = 3;

    private VoxelFace[ , , ] voxels; 
    private VoxelFace face;

    private MeshFilter filter;
    private MeshCollider coll;
    private MeshData mesh;

    
    // Start is called before the first frame update
    void Start()
    {
        voxels = new VoxelFace[ChunkWidth,ChunkHeight,ChunkWidth];
        filter = gameObject.GetComponent<MeshFilter>();
        coll = gameObject.GetComponent<MeshCollider>();

        for (int i = 0; i < ChunkWidth; i++)
        {
            for (int j = 0; j < ChunkHeight; j++)
            {
                for (int k = 0; k < ChunkHeight; k++)
                {
                    if(i > ChunkWidth/2 && i < ChunkWidth*0.75 &&
                       j > ChunkHeight/2 && j < ChunkHeight*0.75 &&
                       k > ChunkHeight/2 && k < ChunkHeight*0.75)
                    {
                        face = new VoxelFace(1);
                    }
                    else if (i == 0)
                    {
                        face = new VoxelFace(2);
                    }
                    else
                    {
                        face = new VoxelFace(3);
                    }

                    voxels[i,j,k] = face;
                    
                }
            }
        }
        
        Greedy();
    }

    private void Greedy()
    {
        int i = 0, j = 0, k = 0, l = 0, w = 0, h = 0, u = 0, v = 0, n = 0, side = 0;

        int[] x = new int[]{0,0,0};
        int[] q = new int[]{0,0,0};
        int[] du = new int[]{0,0,0};
        int[] dv = new int[]{0,0,0};

        VoxelFace[] mask = new VoxelFace[ChunkWidth * ChunkHeight];

        VoxelFace voxelFace, voxelFace1;

        MeshBuilder builder = new MeshBuilder();
        Vector3[] vertices;

        // This is a weird for loop. 
        // backFace is true on the first iteration and false on the second
        // this tracks which direction the indices should run during creation of the quad.
        //
        // This loop runs twice, and the inner loop 3 times - 6 iterations total
        // one for each voxel face.

        for(bool backFace = true, b = false; b != backFace; backFace = backFace && b, b = !b)
        {
            for(int d = 0; d < 3; d++)
            {
                u = (d + 1) % 3;
                v = (d + 2) % 3;

                x[0] = 0;
                x[1] = 0;
                x[2] = 0;

                q[0] = 0;
                q[1] = 0;
                q[2] = 0;
                q[d] = 1;

                // This tracks the side that we're meshing
                if(d == 0)       { side = backFace ? (int)Dir.West : (int)Dir.East; }
                else if (d == 1) { side = backFace ? (int)Dir.Bottom : (int)Dir.Top; }
                else if (d == 2) { side = backFace ? (int)Dir.South : (int)Dir.North; }

                // move through dimention from front to back
                for(x[d] = -1; x[d] < ChunkWidth;)
                {
                    // compute the mask
                    n = 0;

                    for(x[v] = 0; x[v] < ChunkHeight; x[v]++)
                    {
                        for(x[u] = 0; x[u] < ChunkWidth; x[u]++)
                        {
                            // retrieve two voxel faces for comparison
                            voxelFace = (x[d] >= 0)              ? GetVoxelFace(x[0], x[1], x[2], side) : null;
                            voxelFace1 = (x[d] < ChunkWidth - 1) ? GetVoxelFace(x[0] + q[0], x[1] + q[1], x[2] + q[2], side) : null;
                            // overridden equals for voxel face class
                            // 
                            // we choose the face to add to the mask depending on if we're moving through a backface
                            mask[n++] = ((voxelFace is object && voxelFace1 is object && voxelFace.Equals(voxelFace1)))
                                                 ? null : backFace ? voxelFace1 : voxelFace;

                        }
                    }
                    x[d]++;

                    // Generate mesh for the mask
                    n = 0;

                    for(j = 0; j < ChunkHeight; j++)
                    {
                        for(i = 0; i < ChunkWidth;)
                        {
                            if(mask[n] is object)
                            {
                                // compute width
                                for(w = 1; i + w < ChunkWidth && mask[n + w] is object && mask[n + w].Equals(mask[n]); w++) {}

                                // compute height
                                bool done = false;

                                for(h = 1; j + h < ChunkHeight; h++)
                                {
                                    for(k = 0; k < w; k++)
                                    {
                                        if(mask[n + k + h * ChunkWidth] is null || !mask[n + k + h * ChunkWidth].Equals(mask[n]))
                                        {
                                            done = true;
                                            break;
                                        }
                                    }
                                    if( done ) break;
                                }

                                // check transparent
                                if( !mask[n].Transparent )
                                {
                                    x[u] = i;
                                    x[v] = j;

                                    du[0] = 0;
                                    du[1] = 0;
                                    du[2] = 0;
                                    du[u] = w;

                                    dv[0] = 0;
                                    dv[1] = 0;
                                    dv[2] = 0;
                                    dv[v] = h;

                                    // pass mesh data here
                                    vertices = new Vector3[]
                                    {
                                        new Vector3(x[0], x[1], x[2]),
                                        new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]),
                                        new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]),
                                        new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2])
                                    };

                                    vertices[0] *= VoxelSize;
                                    vertices[1] *= VoxelSize;
                                    vertices[2] *= VoxelSize;
                                    vertices[3] *= VoxelSize;

                                    builder.AddSquareFace(vertices, backFace);

                                }
                                //zero out mask
                                for(l = 0; l < h; ++l)
                                {
                                    for(k=0; k < w; ++k)
                                    {
                                        mask[n + k + l * ChunkWidth] = null;
                                    }
                                }

                                // increment and continue
                                i += w;
                                n += w;
                            }
                            else
                            {
                                i++;
                                n++;
                            }
                        }
                    }
                }
            }
        }
        mesh = builder.ToMeshData();
        RenderMesh();
    }

    public VoxelFace GetVoxelFace(int x, int y, int z, int side)
    {        
        VoxelFace voxelFace = voxels[x,y,z];

        voxelFace.Side = side;

        return voxelFace;
    }

    private void RenderMesh()
    {
        filter.ApplyMeshData(mesh);
        // filter.mesh.Clear();
        // // filter.mesh.vertices = meshData.vertices.ToArray();
        // // filter.mesh.triangles = meshData.triangles.ToArray();
        // // filter.mesh.uv = meshData.uv.ToArray();
        // filter.mesh.RecalculateNormals();

        // coll.sharedMesh = null;
        // Mesh mesh = new Mesh();
        // // mesh.vertices = meshData.colVertices.ToArray();
        // // mesh.triangles = meshData.colTriangles.ToArray();
        // mesh.RecalculateNormals();

        // coll.sharedMesh = mesh;
    }
}
