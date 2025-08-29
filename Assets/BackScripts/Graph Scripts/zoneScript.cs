using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class zoneScript : MaskableGraphic
{
    public List<Vector2> points;
    public Vector2 gridSize;

    public Color bottomColor = new Color(0x3B/256f,0x07 / 256f, 0x5C / 256f); 
    public Color topColor = new Color(0x90 / 256f, 0x09 / 256f, 0x1B / 256f);
     
    public float maxY=0;
    private float yStretch,height, width, cellHeight, cellWidth;
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        cellHeight = height / (float)gridSize.y;
        cellWidth = width / (float)gridSize.x;
        yStretch = maxY / (12 - 1);
        vh.Clear();
        drawZone(vh);        
    }

    private void drawZone(VertexHelper vh)
    {
        UIVertex vertex = UIVertex.simpleVert;
        foreach (var point in points)
        {
            vertex.position.x = point.x * cellWidth;
            vertex.position.y = 0;
            vertex.color = bottomColor;
            print(vertex.position);
            vh.AddVert(vertex);
            vertex.position.x = point.x * cellWidth;
            vertex.position.y = point.y * cellHeight;
            vertex.color = topColor;
            print(vertex.position);
            vh.AddVert(vertex);
        }
        for (int i = 0; i < points.Count - 1; i++)
        {
            int index = i * 2;
            vh.AddTriangle(index + 0, index + 1, index + 2);
            vh.AddTriangle(index + 1, index + 2, index + 3);
        }

    }

}
