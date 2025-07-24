using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageShadowController : MonoBehaviour
{
    [Header("Shadow Settings")]
    public Color shadowColor = new Color(0, 0, 0, 0.5f);
    public Vector2 shadowDistance = new Vector2(2, -2);
    public bool useGraphicAlpha = true;

    void Awake()
    {
        // Получаем или добавляем компонент Shadow
        var shadow = GetComponent<Shadow>()
                     ?? gameObject.AddComponent<Shadow>();

        // Прописываем параметры
        shadow.effectColor = shadowColor;
        shadow.effectDistance = shadowDistance;
        shadow.useGraphicAlpha = useGraphicAlpha;
    }
}
