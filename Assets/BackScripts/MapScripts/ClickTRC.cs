using UnityEngine;

public class ClickTRC : MonoBehaviour
{
    [SerializeField]
    private GameObject Minis;

    [SerializeField] private float dragTolerancePixels = 5f;

    private Vector3 pressScreenPos;
    private bool isPressing = false;

    private void OnMouseDown()
    {
        isPressing = true;
        pressScreenPos = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        if (!isPressing) return;
        isPressing = false;

        if (Vector3.Distance(pressScreenPos, Input.mousePosition) <= dragTolerancePixels)
            OnClick();
    }

    private void OnClick() { Minis.SetActive(true); }
    public void CloseMinis() { Minis.SetActive(false); }
}
