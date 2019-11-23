using System;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

public class Chunk
{
    public static readonly Vector3Int Dimensions = new Vector3Int(48,48,48);

    public World World;
    public WorldPos Pos;

    public Vector3Int Position { get; }

    private Block[] blocks { get; }

    public Chunk(Vector3Int position, World parentWorld)
    {
        World = parentWorld;
        Pos = new WorldPos(position.x, position.y, position.z);
        Position = position;
        blocks = new Block[Dimensions.x * Dimensions.y * Dimensions.z];
    }

    public MeshData GenerateMesh()
    {
        MeshBuilder builder = new MeshBuilder();

        bool[,] merged;

        Vector3Int startPos, currPos, quadSize, m, n, offsetPos;
        Vector3[] vertices;

        Block startBlock;

        // Building slices of the mesh 1 plane at a time
        int direction, workAxis1, workAxis2;

        // Iterate over each face of the blocks.        
        for(int face = 0; face < 6; face++)
        {
            bool isBackFace = face > 2;
            direction = face % 3;
            workAxis1 = (direction + 1) % 3;
            workAxis2 = (direction + 2) % 3;

            startPos = new Vector3Int();
            currPos = new Vector3Int();

            // Now iterate over the chunk layer by layer.    
            for( startPos[direction] = 0; startPos[direction] < Dimensions[direction]; startPos[direction]++ )
            {
                merged = new bool[Dimensions[workAxis1], Dimensions[workAxis2]];

                // Build the slices of the mesh.
                //Debug.Log("build slices of mesh");
                
                for( startPos[workAxis1] = 0; startPos[workAxis1] < Dimensions[workAxis1]; startPos[workAxis1]++ )
                {
                    for( startPos[workAxis2] = 0; startPos[workAxis2] < Dimensions[workAxis2]; startPos[workAxis2]++ )
                    {
                        startBlock = GetBlock(startPos);

                        if( merged[startPos[workAxis1], startPos[workAxis2]] 
                            || !startBlock.IsSolid 
                            || !IsBlockFaceVisible(startPos, direction, isBackFace)
                             )
                        {    
                            continue;
                        }

                        // reset the work var
                        quadSize = new Vector3Int();

                        // Figure out the width, then save it                        
                        for( currPos = startPos, currPos[workAxis2]++; currPos[workAxis2] < Dimensions[workAxis2]
                                && CompareStep(startPos, currPos, direction, isBackFace)
                                && !merged[currPos[workAxis1], currPos[workAxis2]]; currPos[workAxis2]++ ) {}
                        quadSize[workAxis2] = currPos[workAxis2] - startPos[workAxis2];

                        // Figure out the height, then save it
                        for(currPos = startPos, currPos[workAxis1]++; currPos[workAxis1] < Dimensions[workAxis1] && CompareStep(startPos, currPos, direction, isBackFace) && !merged[currPos[workAxis1], currPos[workAxis2]]; currPos[workAxis1]++) {
                            for (currPos[workAxis2] = startPos[workAxis2]; currPos[workAxis2] < Dimensions[workAxis2] && CompareStep(startPos, currPos, direction, isBackFace) && !merged[currPos[workAxis1], currPos[workAxis2]]; currPos[workAxis2]++) { }

                            // If we didn't reach the end then its not a good add.
                            if (currPos[workAxis2] - startPos[workAxis2] < quadSize[workAxis2]) {
                                break;
                            } else {
                                currPos[workAxis2] = startPos[workAxis2];
                            }
                        }
                        quadSize[workAxis1] = currPos[workAxis1] - startPos[workAxis1];

                        // Now add the quad to the mesh
                        m = new Vector3Int();
                        m[workAxis1] = quadSize[workAxis1];

                        n = new Vector3Int();
                        n[workAxis2] = quadSize[workAxis2];

                        // Offset when working with front faces.
                        offsetPos = startPos;
                        offsetPos[direction] += isBackFace ? 0 : 1;

                        // Draw the face to the mesh    
                        vertices = new Vector3[]
                        {
                            offsetPos,
                            offsetPos + m,
                            offsetPos + m + n,
                            offsetPos + n
                        };

                        builder.AddSquareFace(vertices, isBackFace);

                        for( int f = 0; f < quadSize[workAxis1]; f++ )
                        {
                            for( int g = 0; g < quadSize[workAxis2]; g++ )
                            {
                                merged[startPos[workAxis1] + f, startPos[workAxis2] + g] = true;
                            }
                        }
                    }
                }
                
            }
        }
        
        return builder.ToMeshData();
    }

    public bool CompareStep(Vector3Int a, Vector3Int b, int direction, bool backFace)
    {
        Block blockA = GetBlock(a);
        Block blockB = GetBlock(b);

        return blockA.Equals(blockB) && blockB.IsSolid && IsBlockFaceVisible(b, direction, backFace);
    }

    public bool IsBlockFaceVisible(Vector3Int blockPosition, int axis, bool backFace)
    {
        blockPosition[axis] += backFace ? -1 : 1;
        return !GetBlock(blockPosition).IsSolid;
    }

    public void SetBlock(Vector3Int index, Block block)
    {
        if(!ContainsIndex(index))
        {
            throw new IndexOutOfRangeException($"Chunk does not contain index: {index}");
        }

        blocks[FlattenIndex(index)] = block;
    }

    public Block GetBlock(Vector3Int index)
    {
        if(!ContainsIndex(index))
        {
            return Block.Air();
        }

        return blocks[FlattenIndex(index)];
    }

    private bool ContainsIndex(Vector3Int index) =>
        index.x >= 0 && index.x < Dimensions.x &&
        index.y >= 0 && index.y < Dimensions.y &&
        index.z >= 0 && index.z < Dimensions.z;

    private int FlattenIndex(Vector3Int index) =>
        (index.z * Dimensions.x * Dimensions.y) +
        (index.y * Dimensions.x) +
        index.x;



}
