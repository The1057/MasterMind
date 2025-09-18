using UnityEngine;

public class clickBuildings : MonoBehaviour
{
    public buildingInfo buildingInfo;

    // Этот метод можно вызывать из других скриптов или самим зданием
    public void OnClick()
    {
        BuildingManager.Instance?.OnBuildingClicked(buildingInfo, transform);
    }
}