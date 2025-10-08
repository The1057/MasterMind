using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class graphSetup : MonoBehaviour
{
    [Header("Graph elements")]
    public GameObject graphDisplay;
    public GameObject grid;
    public GameObject line;
    public GameObject zone;
    [Space(2)]
    public TextMeshProUGUI textNumberPlus2;
    public TextMeshProUGUI textNumberPlus1;
    public TextMeshProUGUI textNumberMinus1;
    public TextMeshProUGUI textNumberMinus2;

    public GraphCanvasScript gridScript;
    public LineScript lineScript;
    public zoneScript zoneScript;

    public storeScript sc;
    public graphData graphData;
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
        //loadGraphFromFile();

        //setup();
    }
    private void Update()
    {
        //if (grid != null)
        //{
        //    grid.transform.position = this.transform.position;
        //    line.transform.position = this.transform.position;
        //    gridScript.gridSize = this.gridSize;
        //    gridScript.thickness = this.gridThickness;
        //    lineScript.gridSize = this.gridSize;
        //    lineScript.lineThickness = this.lineThickness;
        //    lineScript.points = this.points;
        //    zoneScript.points = this.points;
        //    zoneScript.gridSize = this.gridSize;
        //    zoneScript.maxY = this.findMaxY(points);
        //}
    }
    public void setup()
    {
        var tempGraph = new List<Vector2>(graphData.points);

        float maxY = findABSMaxY(graphData.points);
        print($"maxY: {maxY}");
        yStretch = maxY / (graphHeight - 1);
        for (int i = 0; i < tempGraph.Count; i++)
        {
            tempGraph[i] = new Vector2(tempGraph[i].x+1 - (graphHeight / gridSize.x), tempGraph[i].y / yStretch);
        }

        grid.transform.position = this.transform.position;
        line.transform.position = this.transform.position;
        zone.transform.position = this.transform.position;

        gridScript.gridSize = this.gridSize;
        gridScript.thickness = this.gridThickness;
        gridScript.gridColor = this.gridColor;

        lineScript.gridSize = this.gridSize;
        lineScript.lineThickness = this.lineThickness;
        lineScript.points = tempGraph;
        lineScript.lineColor = this.lineColor;

        zoneScript.points = tempGraph;
        zoneScript.gridSize = this.gridSize;
        zoneScript.maxY = this.findABSMaxY(points);
        zoneScript.topColor = this.zoneTopColor;
        zoneScript.bottomColor = this.zoneBottomColor;


        textNumberPlus2.text = this.findABSMaxY(points).ToString();
        textNumberPlus1.text = (this.findABSMaxY(points) / 2).ToString();
        textNumberMinus1.text = (-this.findABSMaxY(points) / 2).ToString();
        textNumberMinus2.text = (-this.findABSMaxY(points)).ToString();
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

        float maxY = findABSMaxY(graphData.points);
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
    private float findABSMaxY(List<Vector2> points)
    {
        float max = -999999f;

        foreach (Vector2 point in points)
        {
            if(max < Mathf.Abs(point.y)) max = point.y;
        }

        return MathF.Abs(max);
    }
    private float findABSMaxYSign(List<Vector2> points)
    {
        float max = -999999f;

        foreach (Vector2 point in points)
        {
            if (max < Mathf.Abs(point.y)) max = point.y;
        }

        return MathF.Sign(max);
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
