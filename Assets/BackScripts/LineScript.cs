using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LineScript : MaskableGraphic
{
    public Vector2 gridSize;
    public List<Vector2> points;

    float width,height;
    float cellWidth,cellHeight;

    public float lineThickness=10f;
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        cellHeight = height/(float)gridSize.y;
        cellWidth = width/(float)gridSize.x;
        if(points.Count < 2) return;
        float angle=0;
        for(int i = 0; i < points.Count; i++)
        {
            Vector2 point = points[i];

            if (i < points.Count - 1)
            {
                angle = getAngle(points[i], points[i+1])+45f;
            }

            drawPoint(point,vh,angle);
        }

        for (int i = 0; i < points.Count-1; i++)
        {
            int index = i * 2;
            vh.AddTriangle(index+0,index+1,index+3);
            vh.AddTriangle(index+3,index+2,index+0);
        }
    }
    void drawPoint(Vector2 point, VertexHelper vh,float angle)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = new Color(1*point.y,1,1);

        vertex.position = Quaternion.Euler(0,0,angle) * new Vector3(-lineThickness/2,0);
        vertex.position += new Vector3(cellWidth*point.x,cellHeight*point.y);
        vh.AddVert(vertex);

        vertex.position = vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(lineThickness / 2, 0);
        vertex.position += new Vector3(cellWidth * point.x, cellHeight * point.y);
        vh.AddVert(vertex);

    }

    private float getAngle(Vector2 me, Vector2 target)
    {
        return (float)(Mathf.Atan2(target.y - me.y, target.x - me.x) * (180 / Mathf.PI));
    }


}
