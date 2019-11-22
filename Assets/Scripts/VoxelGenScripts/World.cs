using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Dictionary<WorldPos, Chunk> chunks = new Dictionary<WorldPos, Chunk>();

    [SerializeField] private GameObject chunkPrefab;

    public void Start()
    {
        for( int x = -2; x < 2; x++ )
        {
            for( int y = -1; y < 1; y++ )
            {
                for( int z = -1; z < 1; z++ )
                {
                    CreateChunk(x * 16, y * 16, z * 16);
                }
            }
        }
    }

    public void CreateChunk(int x, int y, int z)
    {
        WorldPos worldPos = new WorldPos(x, y, z);

        GameObject newChunkObject = Instantiate(chunkPrefab, new Vector3(x, y, z), Quaternion.Euler(Vector3.zero)) as GameObject;

        Chunk newChunk = newChunkObject.GetComponent<Chunk>();
        newChunk.pos = worldPos;
        newChunk.world = this;

        chunks.Add(worldPos, newChunk);

        for(int xi = 0; xi < 16; xi++ )
        {
            for(int yi = 0; yi < 16; yi++)
            {
                for(int zi = 0; zi < 16; zi++)
                {
                    if(yi <= 7 && !(yi % 2 == 0 && xi % 3 == 0) && !(xi % 5 == 0 || yi %  5 == 0))
                    {
                        SetBlock(x+xi, y+yi, z+zi, new BlockGrass());
                    }
                    else
                    {
                        SetBlock(x+xi, y+yi, z+zi, new BlockAir());
                    }
                }
            }
        }
    }

    public void DestroyChunk(int x, int y, int z)
    {
        Chunk chunk = null;
        if( chunks.TryGetValue(new WorldPos(x, y, z), out chunk))
        {
            Object.Destroy(chunk.gameObject);
            chunks.Remove(new WorldPos(x, y, z));
        }
    }

    public Chunk GetChunk(int x, int y, int z)
    {
        float multiple = Chunk.chunkSize; 
        WorldPos pos = new WorldPos(Mathf.FloorToInt(x / multiple) * Chunk.chunkSize,
                                    Mathf.FloorToInt(y / multiple) * Chunk.chunkSize,
                                    Mathf.FloorToInt(z / multiple) * Chunk.chunkSize);
        Chunk containerChunk = null;
        chunks.TryGetValue(pos, out containerChunk);

        return containerChunk;        
    }

    public Block GetBlock(int x, int y, int z)
    {
        Chunk containerChunk = GetChunk(x, y, z);
        if( containerChunk is object )
        {
            Block block = containerChunk.GetBlock(x - containerChunk.pos.x,
                                                  y - containerChunk.pos.y,
                                                  z - containerChunk.pos.z);
            return block;
        }
        else
        {
            return new BlockAir();
        }
    }

    public void SetBlock(int x, int y, int z, Block block)
    {
        Chunk chunk = GetChunk(x, y, z);

        if( chunk is object )
        {
            chunk.SetBlock(x - chunk.pos.x, y - chunk.pos.y, z - chunk.pos.z, block);
            chunk.update = true;
        }
    }
}
