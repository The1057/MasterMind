using UnityEngine;
using UnityEngine.UI;

public class cardManagerScript : MonoBehaviour
{
    public cardContainerScript[,] slotSpace;
    public GridLayoutGroup grid;
    int slotSpaceWidth, slotSpaceHeight;
    void Start()
    {
        slotSpaceWidth = Mathf.CeilToInt(this.gameObject.GetComponent<RectTransform>().rect.width / this.grid.cellSize.x);
        slotSpaceHeight = Mathf.CeilToInt((float)(this.gameObject.transform.childCount) / (float)(slotSpaceWidth));
        slotSpace = new cardContainerScript[slotSpaceHeight,slotSpaceWidth];
        print($"Height: {slotSpaceHeight} width: {slotSpaceWidth}");
    }
    void Update()
    {
    }
}
