using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageContainer : MonoBehaviour
{
    public GameObject objectBeingDragged { get; set; }

    void Awake()
    {
        objectBeingDragged = null;
    }
}
