using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControl : MonoBehaviour
{
    Vector3 touchStart;
    public Camera cam;
    public generateMap map;
    public bool pan = false;
    public float panningSpeed = 1.0f;
    public Vector3 panTo = new Vector3 (10,5,10);

    public int lowBorderMargin = 3, highBorderMargin = 4;
    // Update is called once per frame

    private void Start()
    {
        lowBorderMargin = 0;
        highBorderMargin = 5;
        map = GameObject.Find("Map").GetComponent<generateMap>();
    }
    void Update()
    {
        if (pan)
        {
            panToPosition(panTo);
        }
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = getWorldPos(0);
        }
        
        if (Input.GetMouseButton(0) && checkBorders(transform.position + (touchStart - getWorldPos(0))))
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

    public void panToPosition(Vector3 targetLocation)
    {
        Vector3 direction = (targetLocation - Camera.main.transform.position);
        Vector3 move = direction * Time.deltaTime * panningSpeed;
        //print($"direction: {direction.ToString()}/nTime: {Time.deltaTime}");

        if (Mathf.Abs(direction.x) > 0.05 || Mathf.Abs(direction.z) > 0.05)
        {
            Camera.main.transform.position += move;
        }
        else
        {
            pan = false;
        }
    }

    private bool checkBorders(Vector3 t)
    {
        bool ok = true;

        ok = t.x >= 0 - lowBorderMargin && t.z >= 0 - lowBorderMargin && 
            t.x <= map.mapSize -   highBorderMargin && t.z <= map.mapSize - highBorderMargin;

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
