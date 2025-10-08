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

    int intersectVertIndex;
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
            vh.AddVert(vertex);
            vertex.position.x = point.x * cellWidth;
            vertex.position.y = point.y * cellHeight;
            vertex.color = topColor;
            vh.AddVert(vertex);            
        }
        for (int i = 0; i < points.Count - 1; i++)
        {
            int index = i * 2;
            if (i < points.Count - 2 && (points[i].y * points[i + 1].y < 0))
            {
                Vector2 y0point;
                float m = (points[i].y - points[i + 1].y) / (points[i].x - points[i + 1].x);
                float c = points[i].y - m * points[i].x;
                y0point = new Vector2((-c / m) * cellWidth, 0);

                vertex.position = y0point;
                intersectVertIndex = vh.currentVertCount;
                vertex.color = bottomColor;
                vh.AddVert(vertex);

                vh.AddTriangle(index + 0, index + 1, intersectVertIndex);
                vh.AddTriangle(intersectVertIndex, index + 2, index + 3);
                print($"Solving for intersections, current point: {points[i]}, next point: {points[i + 1]}");

            }
            else
            {
                vh.AddTriangle(index + 0, index + 1, index + 2);
                vh.AddTriangle(index + 1, index + 2, index + 3);
            }

        }

    }

}
