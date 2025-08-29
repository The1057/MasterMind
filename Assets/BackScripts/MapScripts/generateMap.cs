using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;

public class generateMap : MonoBehaviour
{
    public graphSetup debugObj;

    GameObject map;
    //public GameObject underTile;
    public int mapSize = 1;

    public float alpha = 1;

    public List<GameObject> tileModels = new List<GameObject>();
    void Start()
    {
    }

    void Update()
    {
    }
}
