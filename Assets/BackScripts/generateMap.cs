using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class generateMap : MonoBehaviour
{
    public graphSetup debugObj;

    GameObject map;
    //public GameObject underTile;
    public int mapSize = 1;
    public float tileSize = 1;

    public int edgeSize = 50;

    public List<GameObject> tileModels = new List<GameObject>();

    private byte[,] mapData = { 
        {1, 0, 1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0}, 
        {0, 1, 1, 0, 0, 2, 2, 1, 0, 1, 0, 0, 0, 0}, 
        {0, 1, 1, 0, 0, 3, 3, 1, 0, 1, 0, 1, 1, 0}, 
        {0, 0, 0, 0, 0, 3, 3, 0, 0, 0, 0, 1, 0, 0}, 
        {0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0}, 
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0},
        {0, 2, 0, 2, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 2, 0, 2, 0, 2, 0, 0, 1, 0, 0, 0, 0, 0},
        {0, 0, 0, 2, 2, 2, 0, 0, 1, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 2, 0, 0, 1, 1, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0},
        {0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2},
        {0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2},
        {0, 2, 2, 0, 0, 0, 0, 2, 0, 2, 0, 0, 0, 0} 
    };

    List<GameObject> underTiles = new List<GameObject>();
    List<GameObject> tiles = new List<GameObject>();
    void Start()
    {
        map = GameObject.Find("Map");
        mapSize = mapData.GetLength(0);

        generator();
    }

    void Update()
    {
    }

    [ContextMenu("Generate test")]
    void homeGenerator()
    {
        //underTiles.Add(Instantiate(underTile, new Vector3(0, 0, 0), transform.rotation));

        tiles.Add(Instantiate(tileModels[0], new Vector3(1,0, 0), transform.rotation));

        tiles.Add(Instantiate(tileModels[1], new Vector3(2, 0, 0), transform.rotation));

        tiles.Add(Instantiate(tileModels[2], new Vector3(3, 0, 0), transform.rotation));

        tiles.Add(Instantiate(tileModels[3], new Vector3(4, 0, 0), transform.rotation));
         
    }


    [ContextMenu("Generate tiles")]
    void generator()
    {
        for (int i = 0; i < mapSize; i++)
        {    
            for(int j = 0; j < mapSize; j++)
            {
                //underTiles.Add(Instantiate(underTile, new Vector3(transform.position.x + i * tileSize,0, transform.position.z + j * tileSize),transform.rotation));

                tiles.Add(Instantiate(tileModels[mapData[i,j]], new Vector3(transform.position.x + i * tileSize, 0, transform.position.z + j * tileSize), transform.rotation));
                                
                tiles.Last().transform.SetParent(map.transform);
                //underTiles.Last().transform.SetParent(map.transform);
            }
        }
        for (int i = -edgeSize; i < mapSize + edgeSize; i++)
        {
            for (int j = -edgeSize; j < mapSize + edgeSize; j++)
            {
                if(!( (i > 0  && j > 0) && (i<mapSize && j < mapSize) ))
                {
                    tiles.Add(Instantiate(tileModels[0], new Vector3(transform.position.x + i * tileSize, 0, transform.position.z + j * tileSize), transform.rotation));
                    tiles.Last().transform.SetParent (map.transform);
                }
            }
        }
    }
    //[ContextMenu("Destroy tiles")]
    //void tileDestroyer()
    //{
    //    foreach (var tile in underTiles)
    //    {
    //        Destroy(tile);
    //    }
    //}


    [ContextMenu("Destroy homes")]
    void homeDestroyer()
    {
        foreach (var home in tiles)
        {
            Destroy(home);
        }
    }
}
