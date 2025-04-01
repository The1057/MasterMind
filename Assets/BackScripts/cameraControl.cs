using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControl : MonoBehaviour
{
    Vector3 touchStart;
    public Camera cam;
    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.DownArrow))
        //{
        //    transform.Rotate(10, 0, 0);
        //}
        //else if (Input.GetKeyDown(KeyCode.UpArrow))
        //{
        //    transform.Rotate(-10, 0, 0);
        //}
        //else if (Input.GetKeyDown(KeyCode.LeftArrow))
        //{
        //    transform.Rotate(0, -10, 0);
        //}
        //else if (Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    transform.Rotate(0, 10, 0);
        //} 
        //else if (Input.GetKeyDown(KeyCode.W))
        //{
        //    transform.position += new Vector3(1,0,0);            
        //}
        //else if (Input.GetKeyDown(KeyCode.S))
        //{
        //    transform.position += new Vector3(-1, 0, 0);
        //}
        //else if (Input.GetKeyDown(KeyCode.A))
        //{
        //    transform.position += new Vector3(0, 1, 0);
        //}
        //else if (Input.GetKeyDown(KeyCode.D))
        //{
        //    transform.position += new Vector3(0, -1, 0);
        //}
        if(Input.GetMouseButtonDown(0))
        {
            touchStart = getWorldPos(0);
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - getWorldPos(0);
            Camera.main.transform.position += direction;
        }
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
