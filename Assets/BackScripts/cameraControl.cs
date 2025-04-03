using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControl : MonoBehaviour
{
    Vector3 touchStart;
    public Camera cam;
    public generateMap map;

    public int lowBorderMargin = 3, highBorderMargin = 4;
    // Update is called once per frame

    private void Start()
    {
        lowBorderMargin = 3;
        highBorderMargin = 4;
        map = GameObject.Find("Map").GetComponent<generateMap>();
    }
    void Update()
    {        
        if(Input.GetMouseButtonDown(0))
        {
            touchStart = getWorldPos(0);
        }
        
        if (Input.GetMouseButton(0) && checkBorders(transform))
        {
            Vector3 direction = touchStart - getWorldPos(0);
            Camera.main.transform.position += direction;
        }
        else if(transform.position.x < 0-lowBorderMargin)
        {
            Camera.main.transform.position += new Vector3(0.1f,0,0);
        } 
        else if(transform.position.z < 0-lowBorderMargin)
        {
            Camera.main.transform.position += new Vector3(0, 0, 0.1f);
        }
        else if (transform.position.x > map.mapSize-highBorderMargin)
        {
            Camera.main.transform.position -= new Vector3(0.1f, 0, 0);
        }
        else if (transform.position.z > map.mapSize-highBorderMargin)
        {
            Camera.main.transform.position -= new Vector3(0, 0, 0.1f);
        }
    }

    private bool checkBorders(Transform t)
    {
        bool ok = true;

        ok = t.position.x >= 0 - lowBorderMargin && t.position.z >= 0 - lowBorderMargin && 
            t.position.x <= map.mapSize -   highBorderMargin && t.position.z <= map.mapSize - highBorderMargin;

        return ok;
    }

    private Vector3 getWorldPos(float y)
    {
        Ray mousePos = cam.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.down,new Vector3(0,y,0));
        float distance;
        ground.Raycast(mousePos, out distance);
        return mousePos.GetPoint(distance);
    }
}
