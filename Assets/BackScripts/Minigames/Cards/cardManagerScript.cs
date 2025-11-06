using UnityEngine;
using UnityEngine.UI;

public class cardManagerScript : MonoBehaviour
{
    public cardContainerScript[,] slotSpace;
    public GridLayoutGroup grid;
    public int slotSpaceWidth, slotSpaceHeight;
    void Start()
    {
        slotSpaceWidth = Mathf.CeilToInt(this.gameObject.GetComponent<RectTransform>().rect.width / this.grid.cellSize.x);
        slotSpaceHeight = Mathf.CeilToInt((float)(this.gameObject.transform.childCount) / (float)(slotSpaceWidth));
        slotSpace = new cardContainerScript[slotSpaceHeight,slotSpaceWidth];
        print($"Height: {slotSpaceHeight} width: {slotSpaceWidth}");

        for(int i = 0; i < slotSpaceHeight; i++)
        {
            for(int j = 0; j < slotSpaceWidth; j++)
            {
                slotSpace[i, j] = transform.GetChild(i*slotSpaceWidth+j).GetComponent<cardContainerScript>();
                slotSpace[i,j].position = new Vector2Int(i,j);
            }
        }
    }
    void Update()
    {
    }
}
