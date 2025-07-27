using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    private graphData graphData;
    public string graphDataDirPath = "";
    public string graphDataFileName = "testGraph.json";

    public Vector2Int gridSize = new Vector2Int(12, 12);
    public List<Vector2> points;
    public float gridThickness = 4f;
    public float lineThickness = 6f;
    public Color lineColor = new Color(1,1,1);
    public Color zoneBottomColor = new Color(0x3B / 256f, 0x07 / 256f, 0x5C / 256f);
    public Color zoneTopColor = new Color(0x90 / 256f, 0x09 / 256f, 0x1B / 256f);
    public Color gridColor = new Color(0.8f,0.8f,0.8f);
    public int graphHeight = 500;

    private float yStretch;
    void Start()
    {
        graphData = new graphData();

        loadGraphFromFile();

        setup();
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
    void setup()
    {
        grid.transform.position = this.transform.position;
        line.transform.position = this.transform.position;
        zone.transform.position = this.transform.position;

        gridScript.gridSize = this.gridSize;
        gridScript.thickness = this.gridThickness;
        gridScript.gridColor = this.gridColor;

        lineScript.gridSize = this.gridSize;
        lineScript.lineThickness = this.lineThickness;
        lineScript.points = this.points;
        lineScript.lineColor = this.lineColor;

        zoneScript.points = this.points;
        zoneScript.gridSize = this.gridSize;
        zoneScript.maxY = this.findMaxY(points);
        zoneScript.topColor = this.zoneTopColor;
        zoneScript.bottomColor = this.zoneBottomColor;
    }

    [ContextMenu("Load graph from file")]
    public void loadGraphFromFile()
    {
        Canvas canvas = GameObject.FindFirstObjectByType<Canvas>();
        // \/ reading graph from file \/
        graphDataDirPath = Application.persistentDataPath;
        string fullPath = Path.Combine(graphDataDirPath, graphDataFileName);
        if (File.Exists(fullPath))
        {
            try
            {
                string rawJSON;
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        rawJSON = reader.ReadToEnd();//magic to read from file
                    }
                }
                graphData = JsonUtility.FromJson<graphData>(rawJSON);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while loading data from file: {fullPath} \n {e}");
            }

            if (graphData == null)
            {
                Debug.LogError("No data found. Creating new instance of saveData");
                graphData = new graphData();
            }
        }
        // /\ reading graph from file /\

        var tempGraph = new List<Vector2>(graphData.points);

        float maxY = findMaxY(graphData.points);
        yStretch = maxY / (graphHeight - 1);
        for (int i = 0; i < tempGraph.Count; i++)
        {
            tempGraph[i] = new Vector2(tempGraph[i].x - (graphHeight / gridSize.x), tempGraph[i].y / yStretch);
        }


        // set graph arguments here \/



        // set graph arguments here /\

        this.gridSize = graphData.gridSize;
        this.points = tempGraph;
        this.gridThickness = graphData.gridThickness;
        this.lineThickness = graphData.lineThickness;
        this.lineColor = graphData.lineColor;
        this.zoneBottomColor = graphData.zoneBottomColor;
        this.zoneTopColor = graphData.zoneTopColor;
        this.gridColor = graphData.gridColor;
        this.graphHeight = graphData.graphHeight;

        Destroy(grid);
        Destroy(line);
        Destroy(zone);

        grid = Instantiate(grid,this.transform);
        line = Instantiate(line, this.transform);
        zone = Instantiate(zone, this.transform);

        gridScript = grid.GetComponent<GraphCanvasScript>();
        lineScript = line.GetComponent<LineScript>();
        zoneScript = zone.GetComponent<zoneScript>();
    }
    public void destroyGraph()
    {
        Destroy(graphDisplay);
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

    [ContextMenu("Save debug graph")]
    public void saveDebugGraphToFile()
    {
        graphDataDirPath = Application.persistentDataPath;
        graphData.points = new List<Vector2>();
        for(int i = 0;i<12;i++)
        {
            graphData.points.Add(new Vector2(i,i));
        }
        // \/ saving example graph to file \/
        string fullPath = Path.Combine(graphDataDirPath, graphDataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            //creating directory 

            string rawJSON = JsonUtility.ToJson(graphData, true);
            //serializing

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(rawJSON);//magic to write to file
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error while saving data from file: {fullPath} \n {e}");
        }
        // /\ saving example graph to file /\
    }
}
