using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public class World : MonoBehaviour
{
    private Stopwatch sw = new Stopwatch();
    //private List<MeshFilter> filter = new List<MeshFilter>(256);
    //private List<MeshCollider> coll = new List<MeshCollider>(256);
    private List<MeshData> chunkMeshes = new List<MeshData>(4096);

    [SerializeField] private GameObject chunkPrefab;
    private List<GameObject> chunkPrefabs = new List<GameObject>(768);
    //public List<Chunk> chunk = new List<Chunk>(16);
    public ConcurrentDictionary<WorldPos, Chunk> chunks = new ConcurrentDictionary<WorldPos, Chunk>();

    private Vector3Int size = new Vector3Int(8,1,8);

    private int chunkCount = 0;

    private ConcurrentBag<Action> instantiations = new ConcurrentBag<Action>();

    void Start()
    {
        //filter.Add(gameObject.GetComponent<MeshFilter>());
        //coll.Add(gameObject.GetComponent<MeshCollider>());
        sw.Start();
        Parallel.For(0, size.x, x =>
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    int worldx = Chunk.Dimensions.x * x;
                    int worldy = Chunk.Dimensions.y * y;
                    int worldz = Chunk.Dimensions.z * z;
                    var worldPosl = new WorldPos(worldx, worldy, worldz);

                    chunks.TryAdd(worldPosl, 
                               new Chunk(new Vector3Int(worldx, worldy, worldz), this));
                    var chunker = chunks[worldPosl];
                    //chunker.SetBlock(new Vector3Int(0, 0, 0), new Block(BlockTypes.Grass, 0));
                    Block grassBlock = new Block(BlockTypes.Grass, 0);
                    Block dirtBlock = new Block(BlockTypes.Dirt, 0);

                    for(int i = 0; i < Chunk.Dimensions.x; i++)
                    {
                        for (int j = 0; j < Chunk.Dimensions.y; j++)
                        {
                            for (int k = 0; k < Chunk.Dimensions.z; k++)
                            {
                                if(i == 0 || j == 0 || k == 0 )
                                {
                                    chunker.SetBlock(new Vector3Int(i, j, k), grassBlock);
                                }
                                else if(i > 32)
                                {
                                    chunker.SetBlock(new Vector3Int(i, j, k), dirtBlock);
                                }
                                
                            }
                        }
                    }
                   
                    // chunks[worldPosl].SetBlock(new Vector3Int(x,y,z), new Block(BlockTypes.Grass, 0));
                    // chunks[worldPosl].SetBlock(new Vector3Int(x+1,y,z), new Block(BlockTypes.Grass, 0));
                    // chunks[worldPosl].SetBlock(new Vector3Int(x+2,y,z), new Block(BlockTypes.Dirt, 0));
                    var mesh = chunker.GenerateMesh();
                    
                    instantiations.Add(() => makeGameObjects(new Vector3(worldx, worldy, worldz), mesh));

                    // chunks.AddThen(new WorldPos(worldx, worldy, worldz), 
                    //                new Chunk(new Vector3Int(worldx, worldy, worldz), this))
                    //                [worldPosl].GenerateMesh();
                    chunkCount++;
                    //Debug.Log(chunkCount);
                    
                        
                }
            }

        });

        foreach (Action v in instantiations)
        {
            v();
        }

        for (int i = 0; i < chunkCount; i++)
        {

            chunkPrefabs[i].GetComponent<MeshFilter>().ApplyMeshData(chunkMeshes[i]);
        }
        sw.Stop();
        UnityEngine.Debug.Log(sw.ElapsedMilliseconds + "ms");
    }

    private void makeGameObjects(Vector3 location, MeshData mesh)
    {
            chunkMeshes.Add(mesh);
            var makeNew = Instantiate(chunkPrefab, location, Quaternion.Euler(Vector3.zero));
            makeNew.transform.SetParent(this.transform);
            chunkPrefabs.Add(makeNew);
    }

}
