using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DestroyByTap : MonoBehaviour
{
    public float destructionTime;
    public bool isOn = false;
    float timer = 0;
    void Update()
    {
        if (isOn)
        {
            timer += Time.deltaTime;

            var color = gameObject.GetComponent<Image>().color;
            color.a = (destructionTime - timer) / destructionTime;
            gameObject.GetComponent<Image>().color = color;

            color = gameObject.GetComponentInChildren<TextMeshProUGUI>().color;
            color.a = (destructionTime - timer) / destructionTime;
            gameObject.GetComponentInChildren<TextMeshProUGUI>().color = color;

            if (timer >= destructionTime)
            {
                Destroy(gameObject);
            }
        }
    }

    public void onClick()
    {
        if (!isOn)
        {
            timer = 0;
            isOn = true;
        }
    }
}