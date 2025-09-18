using UnityEngine;

public class BuildingDetected : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // À Ã
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                clickBuildings building = hit.transform.GetComponent<clickBuildings>();
                if (building != null)
                {
                    building.OnClick();
                }
            }
        }
    }
}