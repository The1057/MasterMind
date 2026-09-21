using UnityEngine;

public class infoButMarket : MonoBehaviour
{
    [SerializeField] private GameObject info;

    private bool yn;

    void Start()
    {
        yn = false;
    }
    public void openOrClose()
    {
        yn = !yn;
        info.SetActive(yn);
    }
}
