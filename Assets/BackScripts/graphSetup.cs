using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class graphSetup : MonoBehaviour
{
    public GameObject graphDisplay;
    public GameObject grid;
    public GameObject line;
    public GameObject zone;

    public GraphCanvasScript gridScript;
    public LineScript lineScript;
    public zoneScript zoneScript;

    public storeScript sc;


    public Vector2Int gridSize = new Vector2Int(12, 12);
    public List<Vector2> points;
    public float gridThickness = 0.75f;
    public float lineThickness = 2.5f;
    public Color lineColor = Color.red;
    public int graphHeight = 300;


    private float yStretch;
    void Start()
    {
        grid.transform.position = this.transform.position;        
        line.transform.position = this.transform.position;
        gridScript.gridSize = this.gridSize;
        gridScript.thickness = this.gridThickness;
        lineScript.gridSize = this.gridSize;
        lineScript.lineThickness = this.lineThickness;
        lineScript.points = this.points;
        zoneScript.points = this.points;
        zoneScript.gridSize = this.gridSize;
        zoneScript.maxY = this.findMaxY(points);
    }

    private void Update()
    {
        if (grid != null)
        {
            grid.transform.position = this.transform.position;
            line.transform.position = this.transform.position;
            gridScript.gridSize = this.gridSize;
            gridScript.thickness = this.gridThickness;
            lineScript.gridSize = this.gridSize;
            lineScript.lineThickness = this.lineThickness;
            lineScript.points = this.points;
            zoneScript.points = this.points;
            zoneScript.gridSize = this.gridSize;
            zoneScript.maxY = this.findMaxY(points);
        }
    }

    public void setup()
    {
        Canvas canvas = GameObject.FindFirstObjectByType<Canvas>();
        sc = GameObject.FindFirstObjectByType<storeScript>();
        
        var tempGraph = new List<Vector2>(sc.points4GraphFinal);

        float maxY = findMaxY(sc.points4GraphFinal);
        yStretch = maxY/(graphHeight-1);
        for(int i = 0; i < tempGraph.Count; i++)
        {
            tempGraph[i] = new Vector2(tempGraph[i].x-(graphHeight/gridSize.x), tempGraph[i].y / yStretch);
        }

        // set graph arguments here \/


        this.lineColor = Color.black;        
        this.points = tempGraph;

        // set graph arguments here /\


        Instantiate(graphDisplay,canvas.transform);
    }

    private float findMaxY(List<Vector2> points)
    {
        float max = -999999f;

        foreach (Vector2 point in points)
        {
            if(max <  point.y) max = point.y;
        }

        return max;
    }

}
